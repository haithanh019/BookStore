    using Microsoft.AspNetCore.Identity;

namespace BookStore.Models
{
    public static class UserRoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<DefaultUser>>();

            string[] roleNames = {"Admin", "User"};
            
            IdentityResult roleResult;

            foreach (var role in roleNames) 
            { 
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists) 
                { 
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));  
                }
            }

            var email = "admin@gmail.com";
            var passE = "1234Qwer@"; 

            if (userManager.FindByEmailAsync(email).Result == null)
            {
                DefaultUser user = new() 
                { 
                Email = email,
                UserName = email,
                FirstName = "Admin",
                LastName = "Adminsson",
                Address = "Ngu Hanh Son",
                City = "Da Nang",
                ZipCode = "550000"
                };
                IdentityResult result = userManager.CreateAsync(user, passE).Result;

                if (result.Succeeded) 
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();

                }
            }

        }
    }
}
