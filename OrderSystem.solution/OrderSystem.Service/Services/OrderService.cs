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
        private readonly IDiscountStrategySelector _discountStrategySelector;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayPalService _payPalPaymentService;
        private readonly ICreditCardService _creditCardPaymentService;

        public OrderService(
            IEmailService emailService,
            IDiscountStrategySelector discountStrategySelector,
            IUnitOfWork unitOfWork,
            IPayPalService payPalPaymentService,
            ICreditCardService creditCardPaymentService)
        {
            _emailService = emailService;
            _discountStrategySelector = discountStrategySelector;
            _unitOfWork = unitOfWork;
            _payPalPaymentService = payPalPaymentService;
            _creditCardPaymentService = creditCardPaymentService;
        }

        public async Task<Order?> PlaceOrderAsync(Order order)
        {
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

            ApplyDiscountsToOrderItems(order);

            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            order.OrderDate = DateTime.Now;
            order.Status = "Pending Payment"; 
            order.InsertDate = DateTime.Now;

            if (order.PaymentMethod == "paypal")
            {
                var apiContext = _payPalPaymentService.GetAPIContext();
                var payment = _payPalPaymentService.CreatePayment(apiContext, order.Id.ToString());
            }
            else if (order.PaymentMethod == "creditcard"){
                var paymentIntent = await _creditCardPaymentService.CreateOrUpdatePaymentIntentId(order);
            }

            await _unitOfWork.Repository<Order>().Add(order);
            _unitOfWork.Repository<Order>().SaveChanges();

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

             _emailService.SendEmail(order.Customer.Email, "Order Placed", $"Your order {order.Id} has been placed.");

            return order;
        }

        public void ApplyDiscountsToOrderItems(Order order)
        {
            if (order == null || order.OrderItems == null) throw new ArgumentNullException();

            decimal orderTotal = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            IDiscountStrategy discountStrategy = _discountStrategySelector.SelectStrategy(orderTotal);
            if (discountStrategy == null) throw new InvalidOperationException("Discount strategy is null.");

            decimal discountPercentage = discountStrategy.GetDiscount(orderTotal);
            order.TotalAmount = orderTotal - (orderTotal * discountPercentage);

            foreach (var item in order.OrderItems)
            {
                decimal itemTotalPrice = item.Quantity * item.UnitPrice;
                decimal itemDiscount = itemTotalPrice * discountPercentage;
                item.UnitPrice -= item.UnitPrice * discountPercentage;
                item.Discount = discountPercentage;
            }
        }


        public async Task<Order?> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var spec = new BaseSpecifications<Order>(o => o.Id == orderId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (order == null)
            {
                return null;
            }

            order.Status = newStatus;
            _unitOfWork.Repository<Order>().Update(order);
            _unitOfWork.Repository<Order>().SaveChanges();
            order.Customer = await _unitOfWork.Repository<Customer>().GetEntityWithSpecAsync(new BaseSpecifications<Customer>(c => c.Id == order.CustomerId));
            _emailService.SendEmail(order.Customer.Email, "Order Status Updated", $"Your order {order.Id} status has been updated to {newStatus}.");

            return order;
        }

        public async Task<Order?> CompletePaymentAsync(string payerId, string paymentId)
        {
            var apiContext = _payPalPaymentService.GetAPIContext();
            var payment = _payPalPaymentService.ExecutePayment(apiContext, payerId, paymentId);

            if (payment.state.ToLower() != "approved")
            {
                return null; 
            }

            var orderId = int.Parse(payment.transactions[0].item_list.items[0].sku);
            var order = await _unitOfWork.Repository<Order>()
                        .GetEntityWithSpecAsync(new BaseSpecifications<Order>(o => o.Id == orderId));
            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            order.Status = "Order Paid"; 
            _unitOfWork.Repository<Order>().Update(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            //await _emailService.SendEmailAsync(order.CustomerEmail, "Order Payment Completed", $"Your order {order.Id} has been paid.");

            return order;
        }


    }
}
