using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Services
{
    public interface IPayPalService
    {
        string GetAccessToken();
        APIContext GetAPIContext();
        Payment CreatePayment(APIContext apiContext, string blogId);
        Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId);
    }
}
