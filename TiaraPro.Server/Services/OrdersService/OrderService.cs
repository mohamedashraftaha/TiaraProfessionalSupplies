using System.Text;
using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Services.EmailService;
using System.Globalization;
using TiaraPro.Server.Services.UsersService;
using TiaraPro.Server.Services.ProductsService;
using TiaraPro.Server.Services.TiaraDentalTraining;
namespace TiaraPro.Server.Services.OrdersService;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;
    private readonly IProductService _productService;
    private readonly IEmailHandler _emailService;
    private readonly IUserService _userService;
    private readonly IDentalTraining _dentalTrainingService;
    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, IProductService productService, IEmailHandler emailService, IUserService userService, IDentalTraining dentalTraining)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _productService = productService;
        _emailService = emailService;
        _userService = userService;
        _dentalTrainingService = dentalTraining;
    }
    public async Task<bool> CreateOrderAsync(Order order)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            int rowsAffected = await _unitOfWork.Orders.CreateOrderAsync(order);

            if (rowsAffected > 0)
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }

            await _unitOfWork.RollbackAsync();
            _logger.LogWarning("No rows affected when creating order. Order ID: {OrderId}", order.Id);
            return false;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error creating order for ID: {OrderId}", order.Id);
            return false;
        }
    }

    public async Task<bool> ConfirmOrderAsync(int orderId, string status)
    {
        try
        {

            bool tiaraAI = false;
            bool tiaraDentalTraining = false;
            #region update stock levels
            var orderDetails = await GetOrderByIdAsync(orderId);

            if (orderDetails == null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                return false;
            }

            await _unitOfWork.BeginTransactionAsync();

            foreach (var item in orderDetails.OrderItems!)
            {
                var product = await _unitOfWork.Products.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    if (product.Quantity >= item.Quantity)
                    {
                        var updated = await _productService.UpdateProductStockLevel(product, -item.Quantity, item.ProductName!);
                        if (!updated)
                        {
                            _logger.LogWarning("Failed to update stock for product ID: {ProductId}", product.Id);
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Insufficient stock for product ID: {ProductId}. Available: {Available}, Requested: {Requested}",
                            product.Id, product.Quantity, item.Quantity);
                        return false;
                    }
                }
                else
                {
                    if (item.ProductId != 1000000 && item.ProductId != 1000001 && item.ProductId != 1000002 && item.ProductId != 1000003)
                    {
                        if (item.ProductId >= 200000)
                        {
                            tiaraDentalTraining = true;
                            var dentalTrainingId = item.ProductId - 2000000;
                            var dentalTraining = await _dentalTrainingService.GetTrainingByIdAsync(dentalTrainingId);
                            if (dentalTraining != null)
                            {
                                if (dentalTraining.Capacity > 0)
                                {
                                    dentalTraining.Capacity--;
                                    var updated = await _dentalTrainingService.UpdateTrainingAsync(dentalTraining);
                                    if (!updated)
                                    {
                                        _logger.LogWarning("Failed to update capacity for dental training ID: {DentalTrainingId}", dentalTraining.Id);
                                        return false;
                                    }
                                    var confirmRegistration = await _dentalTrainingService.ConfirmRegistrationAsync(orderId, orderDetails.UserId.GetValueOrDefault());
                                    if (!confirmRegistration)
                                    {
                                        _logger.LogWarning("Failed to confirm registration for dental training ID: {DentalTrainingId}", dentalTraining.Id);
                                        return false;
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("Insufficient capacity for dental training ID: {DentalTrainingId}. Available: {Available}, Requested: {Requested}",
                                        dentalTraining.Id, dentalTraining.Capacity, item.Quantity);
                                    return false;
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Dental training not found with ID: {DentalTrainingId}", dentalTrainingId);
                                return false;
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Product not found with ID: {ProductId}", item.ProductId);
                            return false;
                        }

                    }
                    else
                    {
                        tiaraAI = true;
                    }


                    // dental training
                }
            }

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError("Error confirming order with ID: {OrderId} and status: {Status}", orderId, status);
                return false;
            }
            #endregion

        #region Send Email

            var user = await _userService.GetUserByIdAsync(orderDetails.UserId.GetValueOrDefault());


            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", orderDetails.UserId);
            }


            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "OrderConfirmationTemplate.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Generate the items HTML
            var orderItemsHtml = new StringBuilder();
            var egyptCulture = new CultureInfo("ar-EG");
            foreach (var item in orderDetails.OrderItems)
            {
                orderItemsHtml.Append($@" 
        <div class=""item"">
            <strong>{item.ProductName}</strong><br/>
            Quantity: {item.Quantity}<br/>
             Price: {item.Price.ToString("C", egyptCulture)}
        </div>");
            }
            var shippingPrice = 150;
            if (!tiaraAI && !tiaraDentalTraining)
            {
                orderItemsHtml.Append($@" 
                    <div class=""item"">
                        <strong>Shipping</strong><br/>
                        Quantity: 1<br/>
                            Price: {shippingPrice.ToString("C", egyptCulture)}
                    </div>");
            }

            if (user.Id == 0)
            {
                user.FirstName = orderDetails.ShippingUserFirstName;
                user.MiddleName = orderDetails.ShippingUserMiddleName;
                user.LastName = orderDetails.ShippingUserLastName;
                user.Email = orderDetails.ShippingEmail;
            }



            // Replace the placeholders
            htmlTemplate = htmlTemplate
                .Replace("{{CustomerName}}", $"{user!.FirstName} {user.MiddleName} {user.LastName} " ?? "Customer")
                .Replace("{{OrderId}}", orderDetails.Id.ToString())
                .Replace("{{OrderItems}}", orderItemsHtml.ToString())
                .Replace("{{TotalAmount}}", orderDetails.TotalAmount.ToString("C", egyptCulture));

            string ccEmail = "sales@tiarapro.com";
            if (tiaraDentalTraining)
            {
                ccEmail = "dentaltraining@tiarapro.com";
            }
            await _emailService.SendEmailAsync(user.Email, $"Order Confirmation - #{orderDetails.Id}", htmlTemplate, ccEmail);
            #endregion
            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError("Error confirming order with ID: {OrderId} and status: {Status}, {ExceptionMessage}", orderId, status, ex.Message);
            return false;
        }
    }

    public async Task<List<Order>> GetAllOrders()
    {
        try
        {
            var orders = await _unitOfWork.Orders.GetAllOrdersAsync();
            return orders;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders.");
            throw;


        }

    }
    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                return null;
            }
            await _unitOfWork.CompleteAsync();
            return order;

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error retrieving order by ID.");
            throw;
        }
    }
    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                _logger.LogWarning("No orders found for user ID: {UserId}", userId);
                return new List<Order>();
            }
            foreach (var order in orders)
            {
                foreach (var item in order!.OrderItems!)
                {
                    var product = await _unitOfWork.Products.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.Name;
                        item.ProductImage = product.LogoUrl;
                    }
                    else
                    {
                        _logger.LogWarning("Product not found with ID: {ProductId}", item.ProductId);
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
            return orders;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error retrieving orders by user ID.");
            throw;
        }
    }
    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                return false;
            }

            // Only restock if status is being changed to Cancelled and it wasn't already Cancelled
            if (status == "Cancelled" && order.Status != "Cancelled")
            {
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            // Restock the product by the quantity in the order item
                            await _productService.UpdateProductStockLevel(product, item.Quantity, item.ProductName ?? product.Name);
                        }
                    }
                }
            }

            order.Status = status;
            await _unitOfWork.CompleteAsync();
            return order != null;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error updating order status.");
            throw;
        }
    }
    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _unitOfWork.Orders.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                return false;
            }
            await _unitOfWork.Orders.DeleteOrderAsync(orderId);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error deleting order.");
            throw;
        }
    }
}
