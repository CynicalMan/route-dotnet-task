using OrderSystem.Core;
using OrderSystem.Service.DiscountStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Service
{
    public class DiscountStrategySelector : IDiscountStrategySelector
    {
        private readonly IDiscountStrategy _highTierDiscountStrategy;
        private readonly IDiscountStrategy _mediumTierDiscountStrategy;
        private readonly IDiscountStrategy _noDiscountStrategy;

        public DiscountStrategySelector(
            IDiscountStrategy highTierDiscountStrategy,
            IDiscountStrategy mediumTierDiscountStrategy,
            IDiscountStrategy noDiscountStrategy)
        {
            _highTierDiscountStrategy = highTierDiscountStrategy;
            _mediumTierDiscountStrategy = mediumTierDiscountStrategy;
            _noDiscountStrategy = noDiscountStrategy;
        }

        public IDiscountStrategy SelectStrategy(decimal orderTotal)
        {
            if (orderTotal > 200m)
            {
                return _highTierDiscountStrategy;
            }
            else if (orderTotal > 100m)
            {
                return _mediumTierDiscountStrategy;
            }
            else
            {
                return _noDiscountStrategy;
            }
        }
    }
}

