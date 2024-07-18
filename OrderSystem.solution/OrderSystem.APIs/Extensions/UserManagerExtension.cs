using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderSystem.APIs.DTO;
using OrderSystem.Core.Entities.Identity;
using System.Security.Claims;

namespace OrderSystem.APIs.Exstentions
{
    public static class UserManagerExtention
    {

        public static async Task<User?> GetUserByIdAsync(this UserManager<User> userManager, string id)
        {
            var user = await userManager.Users.Where(U => U.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public static async Task<User?> GetUserMainAsync(this UserManager<User> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.Users.Where(U => U.Email == email).FirstOrDefaultAsync();
            return user;
        }
    }
}
