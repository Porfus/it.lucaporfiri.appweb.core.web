using Microsoft.AspNetCore.Identity;

namespace it.lucaporfiri.appweb.core.web.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider) 
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Models.ApplicationUser>>();

            string[] roleNames = { "Coach", "Atleta" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "sportcoach@test.it";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null) 
            {
                adminUser = new Models.ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nome = "SportCoach",
                    Cognome = "Admin"
                };            
                await userManager.CreateAsync(adminUser, "Admin123");
                await userManager.AddToRoleAsync(adminUser, "Coach");

            }
        }
    }
}
