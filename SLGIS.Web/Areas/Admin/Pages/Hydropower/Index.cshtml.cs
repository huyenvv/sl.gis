using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Hydropower
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;

        public IndexModel(ILogger<IndexModel> logger, IHydropowerPlantRepository hydropowerPlantRepository)
        {
            _logger = logger;
            _hydropowerPlantRepository = hydropowerPlantRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            Expression<Func<HydropowerPlant, bool>> predicate = m => true;
            if (!string.IsNullOrEmpty(FilterText))
            {
                predicate = m => m.Name.ToLower().Contains(FilterText.ToLower());
            }

            var computers = _hydropowerPlantRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

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

            await _hydropowerPlantRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted hydropowerPlant {id}");

            return RedirectToPage("./Index");
        }
    }
}
