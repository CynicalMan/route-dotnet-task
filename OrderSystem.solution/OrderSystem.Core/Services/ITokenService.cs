using Microsoft.AspNetCore.Identity;
using OrderSystem.Core.Entities.Identity;

namespace OrderSystem.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User User, UserManager<User> userManager);
    }
}
