using OrderSystem.Core.Entities.Core;

namespace OrderSystem.APIs.DTO.OrderViews
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public string CustomerId { get; set; }
        public Invoice Invoice { get; set; }

    }
}
