using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TiaraPro.Server.Models;
using TiaraPro.Server.PaymentProvider;
using TiaraPro.Server.Services.OrdersService;
using TiaraPro.Server.Services.Payments;
using TiaraPro.Server.Services.PromoCodes;
using TiaraPro.Server.Services.TiaraAI;
using TiaraPro.Server.Services.UsersService;

namespace TiaraPro.Server.Controllers;

[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymobService _paymobService;
    private readonly ILogger<PaymentController> _logger;
    private readonly PaymobOptions _paymobOptions;
    private readonly IPaymentsService _paymentsService;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;
    private readonly ITiaraAISubscriptionService _subscriptionService;
    private readonly IPromoCodeService _promoCodeService;

    public PaymentController(IPaymobService paymobService, ILogger<PaymentController> logger, IOptions<PaymobOptions> options, IPaymentsService paymentsService, IOrderService orderService, ITiaraAISubscriptionService subscriptionService, IPromoCodeService promoCodeService, IUserService userService)
    {
        _paymobService = paymobService;
        _logger = logger;
        _paymobOptions = options.Value;
        _paymentsService = paymentsService;
        _orderService = orderService;
        _subscriptionService = subscriptionService;
        _promoCodeService = promoCodeService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        try
        {
            var payments = await _paymentsService.GetPayments();
            if (payments == null || payments.Count == 0)
            {
                return NotFound("No payments found.");
            }
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("create-intention")]
    public async Task<IActionResult> CreateIntention([FromBody] PaymobIntentionRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }
        try
        {
            #region Paymob Intention Request
            var response = await _paymobService.CreateIntentionAsync(request);
            if (string.IsNullOrEmpty(response?.ClientSecret))
            {
                return BadRequest("Failed to create payment intention.");   

            }
            #endregion

            #region Save Payment Request in DB
            var orderId = response.Extras.CreationExtras.TryGetValue("tiara_order_id", out var tiaraOrderId) ? tiaraOrderId : 0;

            if (orderId.ToString()!.Contains("tiaraai"))
            {
                orderId = orderId.ToString()!.Split('-')[1];
            }

            if (orderId.ToString()!.Contains("tiaradentaltraining"))
            {
                orderId = orderId.ToString()!.Split('-')[1];
            }

            var paymentRequested = await _paymentsService.CreatePaymentAsync(new Payment
            {
                PaymentMethod = "Credit Card",
                PaymentStatus = response.Status,
                TransactionId = response.IntentionOrderId.ToString(),
                CreatedAt = DateTime.UtcNow,
                OrderId = int.TryParse(orderId.ToString(), out int oid) ? oid : 0,
                Amount = (decimal)request.Amount / 100

            });

            if (!paymentRequested) {
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
            var redirectionLink = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_paymobOptions.PublicKey}&clientSecret={response.ClientSecret}";
            return Ok(redirectionLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment intention.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("payment-status/{transactionId}/{status}")]
    public async Task<IActionResult> UpdatePaymentStatus(int transactionId, string status)
    {
        try
        {
            var payment = await _paymentsService.GetPaymentsByTransactionIdAsync(transactionId);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }
            #region Update payment status
            
            var statusUpdated = await _paymentsService.UpdatePaymentStatusAsync(transactionId, status);
            if (!statusUpdated)
            {
                return BadRequest("Failed to update payment status.");
            }
            #endregion

            #region Update Order status
            var updateOrderStatus = await _orderService.UpdateOrderStatusAsync(payment.OrderId, status);
            if (!updateOrderStatus)
            {
                return BadRequest("Failed to update order status.");
            }
            #endregion

            #region Activate TiaraAI Subscription if payment successful
            if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
            {
                var order = await _orderService.GetOrderByIdAsync(payment.OrderId);
                if (order != null && order.UserId.HasValue)
                {
                    await _subscriptionService.ActivateUserSubscriptionAsync(order.UserId.Value, payment.OrderId);
                }
            }
            #endregion

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment status.");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("transaction-callback")]
    public async Task<IActionResult> GetTransactionCallbackResponse()
    {
        try
        {
            await Task.CompletedTask;
            var response = true;
            if (response == null)
            {
                return NotFound("Transaction not found.");
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction callback response.");
            return StatusCode(500, "Internal server error");
        }
    }
}
