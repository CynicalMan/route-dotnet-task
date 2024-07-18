using Microsoft.AspNetCore.Identity;
using OrderSystem.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Repository.Identity
{

    public static class OrderManagementDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@test.com",
            };

            if (userManager.Users.All(u => u.Email != adminUser.Email))
            {
                var result = await userManager.CreateAsync(adminUser, "Pa$$w0rd1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    throw new Exception($"Failed to create admin user: {errorMessages}");
                }
            }
        }

    }
}
