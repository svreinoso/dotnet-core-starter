using Cine.Data;
using Cine.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Citme.Data
{
    public class SeedDatabase
    {
        public static async Task<bool> InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.Migrate();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                    //SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(user, "Welc0me!");

                if (!context.Roles.Any())
                {
                    roleManager.CreateAsync(new IdentityRole { Name = AppRoles.Admin }).Wait();
                    roleManager.CreateAsync(new IdentityRole { Name = AppRoles.User }).Wait();
                }

                userManager.AddToRoleAsync(user, AppRoles.Admin).Wait();
            }
            return true;
        }
    }
}
