using Microsoft.EntityFrameworkCore;
using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service.Services
{
    public class OrderService
    {
        //private readonly OrderManagementDbCotext _context;
        //private readonly IEmailService _emailService;

        //public OrderService(OrderManagementDbCotext context, IEmailService emailService)
        //{
        //    _context = context;
        //    _emailService = emailService;
        //}

        //public async Task<Order> PlaceOrderAsync(Order order)
        //{
        //    // Validate stock
        //    foreach (var item in order.OrderItems)
        //    {
        //        var product = await _context.Products.FindAsync(item.ProductId);
        //        if (product == null || product.Stock < item.Quantity)
        //        {
        //            throw new InvalidOperationException($"Insufficient stock for product {product?.Name ?? item.ProductId.ToString()}.");
        //        }
        //    }

        //    // Apply tiered discounts
        //    decimal total = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
        //    if (total > 200)
        //    {
        //        order.Discount = 0.10m;
        //    }
        //    else if (total > 100)
        //    {
        //        order.Discount = 0.05m;
        //    }
        //    else
        //    {
        //        order.Discount = 0;
        //    }
        //    order.Total = total - (total * order.Discount);

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    var invoice = new Invoice
        //    {
        //        OrderId = order.Id,
        //        Amount = order.Total,
        //        GeneratedAt = DateTime.UtcNow
        //    };
        //    _context.Invoices.Add(invoice);
        //    await _context.SaveChangesAsync();

        //    await _emailService.SendEmailAsync(order.CustomerEmail, "Order Placed", $"Your order {order.Id} has been placed.");

        //    return order;
        //}

        //public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        //{
        //    var order = await _context.Orders.FindAsync(orderId);
        //    if (order == null)
        //    {
        //        throw new InvalidOperationException("Order not found.");
        //    }

        //    order.Status = newStatus;
        //    await _context.SaveChangesAsync();

        //    await _emailService.SendEmailAsync(order.CustomerEmail, "Order Status Updated", $"Your order {order.Id} status has been updated to {newStatus}.");
        //}
    }

}

