using SLGIS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.User
{
    [Authorize(Roles = Constant.Role.Admin + "," + Constant.Role.SupperAdmin)]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<Core.User> _userManager;

        public IndexModel(ILogger<IndexModel> logger, UserManager<Core.User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public async Task OnGet(string searchText = null, int? pageIndex = 1)
        {

            var Users = (await _userManager.GetUsersInRoleAsync(Constant.Role.Member));

            if (User.IsInRole(Constant.Role.SupperAdmin))
            {
                var admins = (await _userManager.GetUsersInRoleAsync(Constant.Role.Admin)).ToList();

                admins.AddRange(Users);
                Users = admins;
            }

            FilterText = searchText;
            if (!string.IsNullOrEmpty(FilterText))
            {
                Users = Users.Where(m => m.UserName.ToLower().Contains(FilterText.ToLower()) || m.Name.ToLower().Contains(FilterText.ToLower())).OrderByDescending(m => m.Updated).ToList();
            }

            var pager = new Pager(Users.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("/User/Index"),
                Items = Users.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Page();
            }

            await _userManager.DeleteAsync(user);
            _logger.LogInformation($"Deleted user {id}");

            return RedirectToPage("/User/Index");
        }
    }
}
