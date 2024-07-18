using OrderSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service.DiscountStrategies
{
    public class HighTierDiscount : IDiscountStrategy
    {
        public decimal GetDiscount(decimal orderTotal)
        {
            return 0.10m;
        }
    }
}
