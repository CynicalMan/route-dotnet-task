using OrderSystem.Core.Entities.Core;
using OrderSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSystem.Service.Services;

namespace OrderSystem.Service.PaymentAdapters
{
    public class PayPalAdapter : IPaymentAdapter
    {
        private readonly PayPalPaymentService _payPalPaymentService;

        public PayPalAdapter(PayPalPaymentService payPalPaymentService)
        {
            _payPalPaymentService = payPalPaymentService;
        }

        public bool ProcessPayment(Order order)
        {
            // Adapt the PayPalPaymentService to the IPaymentAdapter interface
            return false;
        }
    }
}
