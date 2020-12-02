using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Report
{
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IReportRepository _reportRepository;

        public IndexModel(ILogger<IndexModel> logger, IReportRepository reportRepository, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _reportRepository = reportRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public IActionResult OnGet(DateTime? startDate, DateTime? endDate, string searchText = null, int? pageIndex = 1)
        {
            var hydropowerPlantId = GetCurrentHydropower().Id;
            ViewData["HydropowerPlantId"] = hydropowerPlantId;

            FilterText = searchText;
            var list = _reportRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).AsQueryable();
            if (!string.IsNullOrEmpty(FilterText))
            {
                list = list.Where(m => m.Title.Contains(FilterText.ToLower()));
            }

            if (startDate.HasValue)
            {
                list = list.Where(m => m.Created >= startDate);
            }

            if (endDate.HasValue)
            {
                list = list.Where(m => m.Created <= endDate);
            }

            var result = list.OrderByDescending(m => m.Created).AsEnumerable();
            var pager = new Pager(list.Count(), pageIndex);
            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
                Items = list.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _reportRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted report {id}");

            return RedirectToPage("./Index");
        }
    }
}
