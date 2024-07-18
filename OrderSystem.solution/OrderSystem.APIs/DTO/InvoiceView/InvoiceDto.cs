using OrderSystem.Core.Entities.Core;

namespace OrderSystem.APIs.DTO.InvoiceView
{
    public class InvoiceDto
    {
        public decimal TotalAmount { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
    }
}
