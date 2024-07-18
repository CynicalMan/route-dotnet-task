using OrderSystem.Core.Entities.Core;

namespace OrderSystem.APIs.DTO.CustomerViews
{
    public class CustomerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
