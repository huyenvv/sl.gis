using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<Core.User> _signInManager;
        private readonly UserManager<Core.User> _userManager;

        public LoginModel(ILogger<IndexModel> logger, SignInManager<Core.User> signInManager, UserManager<Core.User> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginViewModel LoginInfoModel { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostLogin(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/Computer");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(LoginInfoModel.Username, LoginInfoModel.Password, LoginInfoModel.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(LoginInfoModel.Username);
                if (user.IsLocked)
                {
                    await _signInManager.SignOutAsync();
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa!");
                    return Page();
                }

                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa!");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng. Vui lòng thử lại!");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToPage("./Index");
        }
    }
}