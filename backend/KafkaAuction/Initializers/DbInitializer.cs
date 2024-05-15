using KafkaAuction.Constants;
using KafkaAuction.Models;
using Microsoft.AspNetCore.Identity;

namespace KafkaAuction.Initializers;

public static class DbInitializer
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(RoleConstants.AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleConstants.AdminRole));
        }

        if (!await roleManager.RoleExistsAsync(RoleConstants.UserRole))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleConstants.UserRole));
        }
    }

    public static async Task EnsureUser(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, string userEmail, string userPassword, string roleName)
    {
        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new UserModel
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, userPassword);
        }

        // Ensure the role exists
        await SeedRoles(roleManager);

        // Assign the role to the user if not already assigned
        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}