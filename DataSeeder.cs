using Microsoft.AspNetCore.Identity;

public static class DataSeeder
{
    public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
    {
        if (await userManager.FindByNameAsync("testuser") == null)
        {
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com"
            };
            await userManager.CreateAsync(user, "Test@123");
        }
    }
}
