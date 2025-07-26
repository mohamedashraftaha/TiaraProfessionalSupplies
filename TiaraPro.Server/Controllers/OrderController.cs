using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.OrdersService;

namespace TiaraPro.Server.Controllers;

[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        if (order == null)
        {
            return BadRequest("Invalid order data.");
        }
        var result = await _orderService.CreateOrderAsync(order);

        if (result)
        {
            return CreatedAtAction(nameof(CreateOrder), new { orderid = order.Id });
        }
        return BadRequest("Failed to create order.");
    }


    [HttpPost("confirmOrder/{orderId}/{status}")]
    public async Task<IActionResult> ConfirmOrder(int orderId, string status)
    {
        if (orderId == 0 || string.IsNullOrEmpty(status))
        {
            return BadRequest("Invalid order data.");
        }

        var result = await _orderService.ConfirmOrderAsync(orderId, status);
        if (result)
        {
            return Ok("Order confirmed successfully.");
        }
        return BadRequest("Failed to confirm order.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrders();
        if (orders == null || orders.Count == 0)
        {
            return NotFound("No orders found.");
        }
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found.");
        }
        return Ok(order);
    }
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUser(int userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }
    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
        if (result)
        {
            return Ok("Order status updated successfully.");
        }
        return BadRequest("Failed to update order status.");
    }
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var result = await _orderService.DeleteOrderAsync(orderId);
        if (result)
        {
            return Ok("Order deleted successfully.");
        }
        return BadRequest("Failed to delete order.");
    }
}
