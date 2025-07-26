using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.OrdersService
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetAllOrders();

        Task<bool> ConfirmOrderAsync(int orderId, string status);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
