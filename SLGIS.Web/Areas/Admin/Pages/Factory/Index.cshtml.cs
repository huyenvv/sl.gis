using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Factory
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFactoryRepository _factoryRepository;

        public IndexModel(ILogger<IndexModel> logger, IFactoryRepository factoryRepository)
        {
            _logger = logger;
            _factoryRepository = factoryRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            Expression<Func<Core.Factory, bool>> predicate = m => true;
            if (!string.IsNullOrEmpty(FilterText))
            {
                predicate = m => m.Title.ToLower().Contains(FilterText.ToLower());
            }

            var computers = _factoryRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

            var pager = new Pager(computers.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
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

            await _factoryRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted factory {id}");

            return RedirectToPage("./Index");
        }
    }
}
