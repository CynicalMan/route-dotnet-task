using OrderSystem.Core.Entities.Core;

namespace OrderSystem.APIs.DTO.OrderViews
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Invoice Invoice { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
