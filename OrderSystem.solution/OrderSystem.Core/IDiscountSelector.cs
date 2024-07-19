using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core
{
    public interface IDiscountStrategySelector
    {
        IDiscountStrategy SelectStrategy(decimal orderTotal);
    }
}
