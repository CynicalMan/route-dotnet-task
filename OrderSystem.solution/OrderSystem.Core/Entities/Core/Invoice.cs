using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Entities.Core
{
    public class Invoice : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

    }
}
