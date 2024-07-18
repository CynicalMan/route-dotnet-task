using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Entities.Core
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Invoice Invoice { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
