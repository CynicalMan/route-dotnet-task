using Moq;
using Xunit;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Services;
using OrderSystem.Service.Services;
using OrderSystem.Core.Specifications;
using System.Threading.Tasks;
using OrderSystem.Core;
using PayPal.Api;

namespace OrderSystem.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IDiscountStrategySelector> _mockDiscountStrategySelector;
        private readonly Mock<IDiscountStrategy> _mockHighTierDiscountStrategy;
        private readonly Mock<IDiscountStrategy> _mockMediumTierDiscountStrategy;
        private readonly Mock<IDiscountStrategy> _mockNoDiscountStrategy;
        private readonly Mock<IPayPalService> _mockPayPalPaymentService;
        private readonly Mock<ICreditCardService> _mockCreditCardPaymentService;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEmailService = new Mock<IEmailService>();

            _mockHighTierDiscountStrategy = new Mock<IDiscountStrategy>();
            _mockMediumTierDiscountStrategy = new Mock<IDiscountStrategy>();
            _mockNoDiscountStrategy = new Mock<IDiscountStrategy>();

            _mockDiscountStrategySelector = new Mock<IDiscountStrategySelector>();
            _mockDiscountStrategySelector
                .Setup(selector => selector.SelectStrategy(It.IsAny<decimal>()))
                .Returns<decimal>(total =>
                {
                    if (total > 200m) return _mockHighTierDiscountStrategy.Object;
                    else if (total > 100m) return _mockMediumTierDiscountStrategy.Object;
                    else return _mockNoDiscountStrategy.Object;
                });

            _mockHighTierDiscountStrategy.Setup(ds => ds.GetDiscount(It.IsAny<decimal>())).Returns(0.2m);
            _mockMediumTierDiscountStrategy.Setup(ds => ds.GetDiscount(It.IsAny<decimal>())).Returns(0.1m);
            _mockNoDiscountStrategy.Setup(ds => ds.GetDiscount(It.IsAny<decimal>())).Returns(0.0m);

            _mockPayPalPaymentService = new Mock<IPayPalService>();
            _mockCreditCardPaymentService = new Mock<ICreditCardService>();

            _orderService = new OrderService(
                _mockEmailService.Object,
                _mockDiscountStrategySelector.Object,
                _mockUnitOfWork.Object,
                _mockPayPalPaymentService.Object,
                _mockCreditCardPaymentService.Object
            );
        }

        [Fact]
        public async Task PlaceOrderAsync_ShouldReturnNull_WhenInsufficientStock()
        {
            // Arrange
            var order = new Core.Entities.Core.Order
            {
                OrderItems = new List<OrderItem>
        {
            new OrderItem { ProductId = 1, Quantity = 10 }
        }
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<Product>().GetEntityWithSpecAsync(It.IsAny<BaseSpecifications<Product>>()))
                .ReturnsAsync(new Product { Id = 1, Stock = 5 });

            // Act
            var result = await _orderService.PlaceOrderAsync(order);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task PlaceOrderAsync_ShouldPlaceOrder_WhenStockIsSufficient()
        {
            // Arrange
            var order = new Core.Entities.Core.Order
            {
                CustomerId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 5, UnitPrice = 100 }
                },
                PaymentMethod = "paypal"
            };

            var product = new Product { Id = 1, Stock = 10, Price = 100 };
            var customer = new Customer { Id = 1, Name = "John Doe" };

            _mockUnitOfWork.Setup(uow => uow.Repository<Product>().GetEntityWithSpecAsync(It.IsAny<BaseSpecifications<Product>>()))
                .ReturnsAsync(product);

            _mockUnitOfWork.Setup(uow => uow.Repository<Customer>().GetEntityWithSpecAsync(It.IsAny<BaseSpecifications<Customer>>()))
                .ReturnsAsync(customer);

            _mockPayPalPaymentService.Setup(p => p.CreatePayment(It.IsAny<APIContext>(), It.IsAny<string>()))
                .Returns(new Payment());

            _mockUnitOfWork.Setup(uow => uow.Repository<Core.Entities.Core.Order>()
            .Add(It.IsAny<Core.Entities.Core.Order>())).Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.Repository<Core.Entities.Core.Invoice>()
            .Add(It.IsAny<Core.Entities.Core.Invoice>())).Returns(Task.CompletedTask);


            // Act
            var result = await _orderService.PlaceOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pending Payment", result.Status);
        }


        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var newStatus = "Shipped";
            var order = new Core.Entities.Core.Order
            {
                Id = orderId,
                Status = "Pending",
                CustomerId = 1
            };
            var customer = new Customer
            {
                Id = 1,
                Email = "customer@example.com"
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<Core.Entities.Core.Order>().GetEntityWithSpecAsync(It.IsAny<BaseSpecifications<Core.Entities.Core.Order>>()))
                 .ReturnsAsync(order);
            _mockUnitOfWork.Setup(uow => uow.Repository<Customer>().GetEntityWithSpecAsync(It.IsAny<BaseSpecifications<Customer>>()))
                .ReturnsAsync(customer);

            _mockUnitOfWork.Setup(uow => uow.Repository<Core.Entities.Core.Order>()
            .Update(It.IsAny<Core.Entities.Core.Order>()));
            _mockUnitOfWork.Setup(uow => uow.Repository<Core.Entities.Core.Order>().SaveChanges());

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newStatus, result.Status);
            _mockEmailService.Verify(es => es.SendEmail(customer.Email, "Order Status Updated", $"Your order {orderId} status has been updated to {newStatus}."), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Repository<Core.Entities.Core.Order>().Update(It.Is<Core.Entities.Core.Order>(o => o.Id == orderId && o.Status == newStatus)), Times.Once);
        }
    }
}
