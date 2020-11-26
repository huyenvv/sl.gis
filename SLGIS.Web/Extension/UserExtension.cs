using SLGIS.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SLGIS.Web
{
    public static class UserExtension
    {
        public static UserModel ToUserModel(this User user)
        {
            if (user == null) return null;

            return new UserModel
            {
                Id = user.Id.ToString(),
                Username = user.UserName,
                Name = user.Name,
                IsLocked = user.IsLocked
            };
        }

        public static AccountModel ToAccountModel(this User user)
        {
            if (user == null) return null;

            return new AccountModel
            {
                Username = user.UserName,
                Name = user.Name,
            };
        }

        public static Task<User> GetCurrentUser(this IUserRepository userRepository, ClaimsPrincipal user)
        {
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userRepository.GetAsync(m => m.Id == new ObjectId(userId));
        }
        
        public static ObjectId GetId(this ClaimsPrincipal user)
        {
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return new ObjectId(userId);
        }

        public static async Task InitUserAndRoles(this IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();

            foreach (string roleName in Constant.Role.All)
            {
                bool roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new Role(roleName));
                }
            }

            var users = new List<User>()
            {
                new User { UserName = Constant.User.Admin, Email = Constant.User.Admin, Name = Constant.User.Admin },
            };

            foreach (var user in users)
            {
                if (await UserManager.FindByEmailAsync(user.UserName) == null)
                {
                    await UserManager.CreateAsync(user, "Admin123");
                    await UserManager.AddToRoleAsync(user, Constant.Role.SupperAdmin);
                }
            }
        }
    }
}
