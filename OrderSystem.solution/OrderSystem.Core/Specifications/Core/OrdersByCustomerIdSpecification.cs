using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Specifications.Core
{
    public class OrdersByCustomerIdSpecification : BaseSpecifications<Order>
    {
        public OrdersByCustomerIdSpecification(int customerId)
            : base(o => o.CustomerId == customerId)
        {
            Includes.Add(o => o.OrderItems);
            AddOrderby(o => o.OrderDate);
        }
    }
}
