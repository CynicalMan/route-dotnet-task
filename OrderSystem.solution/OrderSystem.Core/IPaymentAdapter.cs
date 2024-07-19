using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core
{
    public interface IPaymentAdapter
    {
        bool ProcessPayment(Order order);
    }
}
