using SLGIS.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.User
{
    public class MyAccountModel : PageModel
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<Core.User> _userManager;

        public MyAccountModel(ILogger<NewOrEditModel> logger, IUserRepository userRepository, UserManager<Core.User> userManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [BindProperty]
        public AccountModel UserModel { get; set; } = new AccountModel();

        public async Task<IActionResult> OnGet(bool isUpdated = false)
        {
            ViewData["IsUpdated"] = isUpdated;

            var user = await _userRepository.GetCurrentUser(User);
            UserModel = user.ToAccountModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userRepository.GetCurrentUser(User);

            if (UserModel.IsChangePassword)
            {
                var correctPassword = await _userManager.CheckPasswordAsync(user, UserModel.OldPassword);
                if (!correctPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không đúng");
                    return Page();
                }

                await _userManager.ChangePasswordAsync(user, UserModel.OldPassword, UserModel.NewPassword);
            }

            user.Name = UserModel.Name;
            user.Updated = DateTime.Now;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"Updated user {UserModel.Username}");
            return RedirectToPage("/User/MyAccount", new { isUpdated = true });
        }
    }
}
