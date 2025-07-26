using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.OrderItems;

public interface IOrderItemsRepository
{
    Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId);
    Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
    Task<int> CreateOrderItemAsync(OrderItem orderItem);
    Task<bool> UpdateOrderItemAsync(OrderItem orderItem);
    Task<bool> DeleteOrderItemAsync(int orderItemId);
}
