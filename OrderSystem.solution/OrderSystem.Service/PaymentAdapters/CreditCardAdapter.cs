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
    public class CreditCardAdapter : IPaymentAdapter
    {
        private readonly CreditCardPaymentService _creditCardPaymentService;

        public CreditCardAdapter(CreditCardPaymentService creditCardPaymentService)
        {
            _creditCardPaymentService = creditCardPaymentService;
        }

        public bool ProcessPayment(Order order)
        {
            // Adapt the CreditCardPaymentService to the IPaymentAdapter interface
            return true;
        }
    }
}
