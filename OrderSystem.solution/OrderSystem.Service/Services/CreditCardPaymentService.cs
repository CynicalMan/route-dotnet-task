using Microsoft.Extensions.Configuration;
using OrderSystem.Core.Repositories;
using Stripe.Terminal;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSystem.Core.Entities.Core;

namespace OrderSystem.Service.Services
{
    public class CreditCardPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public CreditCardPaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["StripeKeys:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }


        public async Task<PaymentIntent> CreateOrUpdatePaymentIntentId(Order order)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            
            order.InsertDate = DateTime.Now;

            return paymentIntent;
        }
    }
}
