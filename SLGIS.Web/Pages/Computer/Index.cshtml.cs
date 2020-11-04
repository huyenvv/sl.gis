using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Computer
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IComputerRepository _computerRepository;

        public IndexModel(ILogger<IndexModel> logger, IComputerRepository computerRepository)
        {
            _logger = logger;
            _computerRepository = computerRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            Expression<Func<Core.Computer, bool>> predicate = m => m.UpdatedBy == User.Identity.Name;
            if (!string.IsNullOrEmpty(FilterText))
            {
                predicate = m => m.Title.ToLower().Contains(FilterText.ToLower()) || m.IpAddress.ToLower().Contains(FilterText.ToLower());
            }

            var computers = _computerRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

            var pager = new Pager(computers.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("/Computer/Index"),
                Items = computers.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _computerRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted computer {id}");

            return RedirectToPage("/Computer/Index");
        }
    }
}
