using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System.Linq;

namespace SLGIS.Web.Pages.User
{
    [Authorize(Roles = Constant.Role.Admin + "," + Constant.Role.SupperAdmin)]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<Core.Role> _roleManager;

        public IndexModel(ILogger<IndexModel> logger, IUserRepository userRepository, RoleManager<Core.Role> roleManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            var roles = _roleManager.Roles.ToList();
            var memeberRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Member).Id.ToString();
            var adminRoleId = roles.FirstOrDefault(m => m.Name == Constant.Role.Admin).Id.ToString();

            var Users = _userRepository.Find(m => m.Roles.Count == 0 || m.Roles.Count == 1 && m.Roles.Contains(memeberRoleId)).ToList();

            if (User.IsInRole(Constant.Role.SupperAdmin))
            {
                var admins = _userRepository.Find(m => m.Roles.Any(n => n == adminRoleId)).ToList();

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
    }
}
