using OrderSystem.Core.Entities.Core;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Services
{
    public interface ICreditCardService
    {
        Task<PaymentIntent> CreateOrUpdatePaymentIntentId(Order order);
    }
}
