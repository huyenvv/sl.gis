using SLGIS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using SLGIS.Core.Extension;

namespace SLGIS.Web.Pages.User
{
    [Authorize(Roles = Constant.Role.Admin + "," + Constant.Role.SupperAdmin)]
    public class NewOrEditModel : PageModel
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<Core.User> _userManager;
        private readonly RoleManager<Core.Role> _roleManager;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IUserRepository userRepository, UserManager<Core.User> userManager, RoleManager<Core.Role> roleManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public RoleRightModel UserModel { get; set; } = new RoleRightModel();

        public IActionResult OnGet(string id)
        {
            var roles = _roleManager.Roles.ToList();
            var memeberRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Member).Id.ToString();
            var adminRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Admin).Id.ToString();

            var user = _userRepository.Find(m => m.Id == new ObjectId(id)).FirstOrDefault();
            UserModel = user.ToRoleRightModel();
            UserModel.IsAdmin = user.Roles.Contains(adminRoleId);
            UserModel.IsMember = user.Roles.Contains(memeberRoleId);
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
                return NotFound();
            }

            return RedirectToPage("/User/Index");
        }

        private async Task UpdateAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var memeberRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Member).Id.ToString();
            var adminRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Admin).Id.ToString();

            if (!User.IsInRole(Constant.Role.SupperAdmin) && !User.IsInRole(Constant.Role.Admin))
                return;

            var user = _userRepository.Find(m => m.Id == new ObjectId(UserModel.Id)).FirstOrDefault();
            user.Updated = DateTime.Now.ToVNDate();
            await _userRepository.UpdateAsync(user);

            if (UserModel.IsMember)
            {
                if (!user.Roles.Contains(memeberRoleId))
                    await _userManager.AddToRoleAsync(user, Constant.Role.Member);
            }
            else
            {
                if (user.Roles.Contains(memeberRoleId))
                    await _userManager.RemoveFromRoleAsync(user, Constant.Role.Member);
            }

            if (!User.IsInRole(Constant.Role.SupperAdmin))
                return;

            if (UserModel.IsAdmin)
            {
                if (!user.Roles.Contains(adminRoleId))
                    await _userManager.AddToRoleAsync(user, Constant.Role.Admin);
            }
            else
            {
                if (user.Roles.Contains(adminRoleId))
                    await _userManager.RemoveFromRoleAsync(user, Constant.Role.Admin);
            }
        }
    }
}
