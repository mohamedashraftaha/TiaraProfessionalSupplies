using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.TiaraAI;
using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Services.ScanTransaction;

namespace TiaraPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiaraAISubscriptionController : ControllerBase
    {
        private readonly ITiaraAISubscriptionService _subscriptionService;
        private readonly IScanTransaction _scanTransaction;
        private readonly ILogger<TiaraAISubscriptionController> _logger;

        public TiaraAISubscriptionController(ITiaraAISubscriptionService subscriptionService, ILogger<TiaraAISubscriptionController> logger, IScanTransaction scanTransaction)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _scanTransaction = scanTransaction;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetAvailableSubscriptions()
        {
            try
            {
                var subscriptions = await _subscriptionService.GetAvailableSubscriptionsAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available subscriptions");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetSubscriptionById(int id)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
                if (subscription == null)
                {
                    return NotFound(new { message = "Subscription plan not found" });
                }
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription by ID: {SubscriptionId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("user/{userId}/subscribe")]
        public async Task<IActionResult> CreateUserSubscription(int userId, [FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var result = await _subscriptionService.CreateUserSubscriptionAsync(userId, request.SubscriptionId, request.OrderId);
                if (result)
                {
                    return Ok(new { message = "Subscription created successfully" });
                }
                return BadRequest(new { message = "Failed to create subscription" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("user/{userId}/activate/{orderId}")]
        public async Task<IActionResult> ActivateUserSubscription(int userId, int orderId)
        {
            try
            {
                var result = await _subscriptionService.ActivateUserSubscriptionAsync(userId, orderId);
                if (result)
                {
                    return Ok(new { message = "Subscription activated successfully" });
                }
                return BadRequest(new { message = "Failed to activate subscription" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<IActionResult> GetActiveUserSubscription(int userId)
        {
            try
            {
                var subscription = await _subscriptionService.GetActiveUserSubscriptionAsync(userId);
                if (subscription == null)
                {
                    return NotFound(new { message = "No active subscription found" });
                }
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active user subscription");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedDefaultSubscriptions()
        {
            try
            {
                var result = await _subscriptionService.SeedDefaultSubscriptionsAsync();
                if (result)
                {
                    return Ok(new { message = "Default subscriptions seeded successfully" });
                }
                return BadRequest(new { message = "Failed to seed subscriptions" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default subscriptions");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("all-with-users")]
        public async Task<IActionResult> GetAllUserSubscriptionsWithUsers()
        {
            var userSubscriptions = await _subscriptionService.GetActiveUserSubscriptionsAsync();
            if (userSubscriptions == null || !userSubscriptions.Any())
            {
                return NotFound(new { message = "No user subscriptions found" });
            }

            var result = new List<object>();
            foreach (var us in userSubscriptions.Where(us => us.IsActive))
            {
                var transactions = await _scanTransaction.GetUserTransactions(us.UserId) ?? new List<TiaraPro.Server.Models.Transactions>();
                result.Add(new {
                    id = us.Id,
                    user_id = us.UserId,
                    user_name = (us.User != null ? ((us.User.FirstName ?? "") + " " + (us.User.LastName ?? "")).Trim() : null),
                    email = us.User?.Email,
                    subscription_id = us.SubscriptionId,
                    subscription_name = us.Subscription?.Name,
                    order_id = us.OrderId,
                    segmentations_used = us.SegmentationsUsed,
                    segmentations_allowed = us.SegmentationsAllowed,
                    subscribed_at = us.SubscribedAt,
                    expires_at = us.ExpiresAt,
                    is_active = us.IsActive,
                    transactions = transactions.Select(t => new {
                        id = t.Id,
                        transaction_guid = t.TransactionGuid,
                        s3_url = t.S3Url,
                        date_created = t.DateCreated,
                        status = t.Status,
                        dental_mesh_response_stl_folder = t.DentalMeshResponseStlFolder ?? string.Empty,
                        dental_mesh_response_stl_viewer = t.DentalMeshResponseStlViewUrl ?? string.Empty,
                    })
                });
            }
            return Ok(result);
        }
    }
} 