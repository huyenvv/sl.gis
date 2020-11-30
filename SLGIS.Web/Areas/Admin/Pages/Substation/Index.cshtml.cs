using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Substation
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISubstationRepository _substationRepository;

        public IndexModel(ILogger<IndexModel> logger, ISubstationRepository substationRepository)
        {
            _logger = logger;
            _substationRepository = substationRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            Expression<Func<Core.Model.Substation, bool>> predicate = m => true;
            if (!string.IsNullOrEmpty(FilterText))
            {
                predicate = m => m.Name.ToLower().Contains(FilterText.ToLower());
            }

            var list = _substationRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

            var pager = new Pager(list.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
                Items = list.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _substationRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted substation {id}");

            return RedirectToPage("./Index");
        }
    }
}
