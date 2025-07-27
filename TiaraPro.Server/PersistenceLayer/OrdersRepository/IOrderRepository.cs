using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.OrdersRepository
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
