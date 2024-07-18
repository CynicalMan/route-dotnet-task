using OrderSystem.Core;
using OrderSystem.Service.PaymentAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service
{
    public class PaymentAdapterFactory
    {
        //public IPaymentAdapter GetPaymentAdapter(string paymentMethod)
        //{
        //    return paymentMethod.ToLower() switch
        //    {
        //        "creditcard" => new CreditCardAdapter(new CreditCardPaymentService()),
        //        "paypal" => new PayPalAdapter(new PayPalPaymentService()),
        //        _ => throw new ArgumentException("Invalid payment method"),
        //    };
        //}
    }

}
