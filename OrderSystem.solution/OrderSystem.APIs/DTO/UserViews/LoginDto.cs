using System.ComponentModel.DataAnnotations;

namespace OrderSystem.APIs.DTO.UserViews
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
    }
}
