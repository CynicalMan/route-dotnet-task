using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OrderSystem.Core.Services;
using PayPal.Api;
using System;
using System.Collections.Generic;

namespace OrderSystem.Service.Services
{
    public class PayPalPaymentService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PayPalPaymentService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetAccessToken()
        {
            var clientId = _configuration.GetValue<string>("PayPal:Key");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");

            return new OAuthTokenCredential(clientId, clientSecret, new Dictionary<string, string> { { "mode", mode } }).GetAccessToken();
        }

        public APIContext GetAPIContext()
        {
            var accessToken = GetAccessToken();
            var mode = _configuration.GetValue<string>("PayPal:mode");
            var apiContext = new APIContext(accessToken) { Config = new Dictionary<string, string> { { "mode", mode } } };
            return apiContext;
        }

        public Payment CreatePayment(APIContext apiContext, string blogId)
        {
            var itemList = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = "Item Detail",
                        currency = "USD",
                        price = "1.00",
                        quantity = "1",
                        sku = blogId
                    }
                }
            };

            var payer = new Payer { payment_method = "paypal" };

            var redirUrls = new RedirectUrls
            {
                cancel_url = _configuration.GetValue<string>("PayPal:RedirectUrls:CancelUrl"),
                return_url = _configuration.GetValue<string>("PayPal:RedirectUrls:ReturnUrl")
            };

            var amount = new Amount
            {
                currency = "USD",
                total = "1.00"
            };

            var transactionList = new List<Transaction>
            {
                new Transaction
                {
                    description = "Transaction description",
                    invoice_number = Guid.NewGuid().ToString(),
                    amount = amount,
                    item_list = itemList
                }
            };

            var payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
