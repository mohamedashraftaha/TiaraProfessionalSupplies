using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.OrderItems
{
    public class OrderItemsRepository : IOrderItemsRepository
    {
        private readonly TiaraDbContext _context;
        public OrderItemsRepository(TiaraDbContext context)
        {
            _context = context;
        }
        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
        {
            return await _context.OrderItems.FindAsync(orderItemId);
        }
        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }
        public async Task<int> CreateOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem.Id;
        }
        public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteOrderItemAsync(int orderItemId)
        {
            var orderItem = await GetOrderItemByIdAsync(orderItemId);
            if (orderItem == null) return false;
            _context.OrderItems.Remove(orderItem);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
