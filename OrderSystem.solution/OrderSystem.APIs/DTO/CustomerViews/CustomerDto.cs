using OrderSystem.Core.Entities.Core;

namespace OrderSystem.APIs.DTO.CustomerViews
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
