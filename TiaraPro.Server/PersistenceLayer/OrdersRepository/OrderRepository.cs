using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.PersistenceLayer.OrdersRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TiaraDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(TiaraDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateOrderAsync(Order order)
        {
            try
            {
                await _context.Orders.AddAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository error creating order with ID: {OrderId}", order.Id);
                throw; // Rethrow to be handled by the service layer
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                // Log the exception
                Console.WriteLine(ex.Message);
                return new List<Order>();
            }
        }
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders
                      .Include(o => o.OrderItems)
                      .Include(o => o.Payments)  // Include payments if needed
                      .FirstOrDefaultAsync(o => o.Id == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order by ID");
                // Log the exception
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(oi => oi.OrderItems)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving orders by user ID");
                Console.WriteLine(ex.Message);
                return new List<Order>();
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await GetOrderByIdAsync(orderId);
                if (order == null) return false;
                order.Status = status;
                _context.Orders.Update(order);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating order status");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await GetOrderByIdAsync(orderId);
                if (order == null) return false;
                _context.Orders.Remove(order);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting order");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
