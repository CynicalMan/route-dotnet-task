using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Specifications.Core
{
    public class OrderWithOrderItemsSpecification : BaseSpecifications<Order>
    {

        public OrderWithOrderItemsSpecification()
            : base()
        {
            Includes.Add(o => o.OrderItems);
            AddOrderby(o => o.OrderDate);
        }

        public OrderWithOrderItemsSpecification(int orderId)
            : base(o => o.Id == orderId)
        {
            Includes.Add(o => o.OrderItems);
            AddOrderby(o => o.OrderDate);
        }
    }
}
