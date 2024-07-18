namespace OrderSystem.APIs.DTO.OrderViews
{
    public class CreateOrderDTO
    {
        public string CustomerId { get; set; }
        public List<CreateOrderItemDTO> OrderItems { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class CreateOrderItemDTO
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
