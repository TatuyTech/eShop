using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data
{
    public class AppDbContextSeed
    {
        public static async Task SeedAsync(AppDbContext identityDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (identityDbContext.Database.IsSqlServer())
            {
                identityDbContext.Database.Migrate();
            }

            await roleManager.CreateAsync(new IdentityRole(Constants.ADMINISTRATORS));

            var defaultUser = new ApplicationUser { UserName = "demouser@microsoft.com", Email = "demouser@microsoft.com" };
            await userManager.CreateAsync(defaultUser, Constants.DEFAULT_PASSWORD);

            string adminUserName = "admin@microsoft.com";
            var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName };
            await userManager.CreateAsync(adminUser, Constants.DEFAULT_PASSWORD);
            adminUser = await userManager.FindByNameAsync(adminUserName);
            await userManager.AddToRoleAsync(adminUser, Constants.ADMINISTRATORS);
        }
    }
}
