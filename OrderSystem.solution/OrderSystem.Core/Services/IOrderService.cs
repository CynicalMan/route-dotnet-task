using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Services
{
    public interface IOrderService
    {
        public Task<Order> PlaceOrderAsync(Order order);
        public Task<Order> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
