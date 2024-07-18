using Microsoft.EntityFrameworkCore;
using OrderSystem.Core;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Services;
using OrderSystem.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IEmailService _emailService;
        private readonly DiscountStrategySelector _discountStrategySelector;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayPalPaymentService _payPalPaymentService;

        public OrderService(
            IEmailService emailService,
            DiscountStrategySelector discountStrategySelector,
            IUnitOfWork unitOfWork,
            PayPalPaymentService payPalPaymentService)
        {
            _emailService = emailService;
            _discountStrategySelector = discountStrategySelector;
            _unitOfWork = unitOfWork;
            _payPalPaymentService = payPalPaymentService;
        }

        public async Task<Order?> PlaceOrderAsync(Order order)
        {
            // Validate stock
            foreach (var item in order.OrderItems)
            {
                var spec = new BaseSpecifications<Product>(p => p.Id == item.ProductId);
                var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
                if (product == null || product.Stock < item.Quantity)
                {
                    return null;
                    //throw new InvalidOperationException($"Insufficient stock for product {product?.Name ?? item.ProductId.ToString()}.");
                }
                product.Stock -= item.Quantity;
                _unitOfWork.Repository<Product>().Update(product);
            }
            _unitOfWork.Repository<Product>().SaveChanges();

            order.Customer = await _unitOfWork.Repository<Customer>().GetEntityWithSpecAsync(new BaseSpecifications<Customer>(c => c.Id == order.CustomerId));

            // Apply tiered discounts
            ApplyDiscountsToOrderItems(order);

            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            order.OrderDate = DateTime.Now;
            order.Status = "Pending Payment"; // Initial status before payment
            order.InsertDate = DateTime.Now;

            // Create PayPal payment
            var apiContext = _payPalPaymentService.GetAPIContext();
            var payment = _payPalPaymentService.CreatePayment(apiContext, order.Id.ToString());


            await _unitOfWork.Repository<Order>().Add(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            // Create and add invoice
            var invoice = new Invoice
            {
                Order = order,
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                InsertDate = DateTime.Now
            };

            await _unitOfWork.Repository<Invoice>().Add(invoice);
            _unitOfWork.Repository<Invoice>().SaveChanges();

            order.Invoice = invoice;

            //await _emailService.SendEmailAsync(order.CustomerEmail, "Order Placed", $"Your order {order.Id} has been placed.");

            return order;
        }

        public void ApplyDiscountsToOrderItems(Order order)
        {
            decimal orderTotal = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            IDiscountStrategy discountStrategy = _discountStrategySelector.SelectStrategy(orderTotal);
            decimal discountPercentage = discountStrategy.GetDiscount(orderTotal);
            order.TotalAmount = orderTotal - (orderTotal * discountPercentage);

            foreach (var item in order.OrderItems)
            {
                decimal itemTotalPrice = item.Quantity * item.UnitPrice;
                decimal itemDiscount = itemTotalPrice * discountPercentage;
                item.UnitPrice -= item.UnitPrice * discountPercentage;
                item.Discount = discountPercentage * 100;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var spec = new BaseSpecifications<Order>(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            order.Status = newStatus;
            _unitOfWork.Repository<Order>().Update(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            //await _emailService.SendEmailAsync(order.CustomerEmail, "Order Status Updated", $"Your order {order.Id} status has been updated to {newStatus}.");
        }

        public async Task<Order?> CompletePaymentAsync(string payerId, string paymentId)
        {
            var apiContext = _payPalPaymentService.GetAPIContext();
            var payment = _payPalPaymentService.ExecutePayment(apiContext, payerId, paymentId);

            if (payment.state.ToLower() != "approved")
            {
                return null; // Payment not approved
            }

            var orderId = int.Parse(payment.transactions[0].item_list.items[0].sku);
            var order = await _unitOfWork.Repository<Order>()
                        .GetEntityWithSpecAsync(new BaseSpecifications<Order>(o => o.Id == orderId));
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            order.Status = "Order Paid"; // Update status after successful payment
            _unitOfWork.Repository<Order>().Update(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            //await _emailService.SendEmailAsync(order.CustomerEmail, "Order Payment Completed", $"Your order {order.Id} has been paid.");

            return order;
        }
    }
}
