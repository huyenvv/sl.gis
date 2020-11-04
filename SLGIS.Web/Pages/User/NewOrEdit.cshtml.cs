using SLGIS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.User
{
    [Authorize(Roles = Constant.Role.Admin + "," + Constant.Role.SupperAdmin)]
    public class NewOrEditModel : PageModel
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<Core.User> _userManager;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IUserRepository userRepository, UserManager<Core.User> userManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [BindProperty]
        public UserModel UserModel { get; set; } = new UserModel();

        public async Task<IActionResult> OnGet(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userRepository.GetById(id);
                UserModel = user.ToUserModel();

                if (User.IsInRole(Constant.Role.SupperAdmin))
                {
                    UserModel.IsAdmin = await _userManager.IsInRoleAsync(user, Constant.Role.Admin);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrEmpty(UserModel.Id))
            {
                await UpdateAsync();
                _logger.LogInformation($"Updated user {UserModel.Username}");
            }
            else
            {
                await CreateAsync();
                _logger.LogInformation($"Created user {UserModel.Username}");
            }

            return RedirectToPage("/User/Index");
        }

        private async Task UpdateAsync()
        {
            var user = await _userRepository.GetById(UserModel.Id);

            user.Name = UserModel.Name;
            user.IsLocked = UserModel.IsLocked;
            user.Updated = DateTime.Now;

            await _userManager.UpdateAsync(user);

            if (UserModel.IsChangePassword)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, UserModel.Password);
            }

            if (!User.IsInRole(Constant.Role.SupperAdmin))
                return;

            var currentIsAdmin = await _userManager.IsInRoleAsync(user, Constant.Role.Admin);
            if (currentIsAdmin && !UserModel.IsAdmin)
            {
                await _userManager.AddToRoleAsync(user, Constant.Role.Member);
                await _userManager.RemoveFromRoleAsync(user, Constant.Role.Admin);
                return;
            }

            if (!currentIsAdmin && UserModel.IsAdmin)
            {
                await _userManager.AddToRoleAsync(user, Constant.Role.Admin);
                await _userManager.RemoveFromRoleAsync(user, Constant.Role.Member);
            }
        }

        private async Task CreateAsync()
        {
            var user = new Core.User
            {
                Name = UserModel.Name,
                IsLocked = UserModel.IsLocked,
                Updated = DateTime.Now,
                UserName = UserModel.Username,
                Email = UserModel.Username
            };

            await _userManager.CreateAsync(user, UserModel.Password);

            var role = Constant.Role.Member;
            if (User.IsInRole(Constant.Role.SupperAdmin) && UserModel.IsAdmin)
            {
                role = Constant.Role.Admin;
            }
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
