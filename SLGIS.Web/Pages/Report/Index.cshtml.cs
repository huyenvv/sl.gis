using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Report
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IReportRepository _reportRepository;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;

        public IndexModel(ILogger<IndexModel> logger, IReportRepository reportRepository, IHydropowerPlantRepository hydropowerPlantRepository)
        {
            _logger = logger;
            _reportRepository = reportRepository;
            _hydropowerPlantRepository = hydropowerPlantRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public IActionResult OnGet(Guid? hydropowerPlantId, DateTime? startDate, DateTime? endDate, string searchText = null, int? pageIndex = 1)
        {
            var hydropowerPlants = ListFactories();
            if (hydropowerPlants.Count == 0 || hydropowerPlantId.HasValue && !hydropowerPlants.Any(m => m.Id == hydropowerPlantId))
            {
                return NotFound();
            }

            hydropowerPlantId ??= hydropowerPlants.First().Id;
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

        private List<Core.HydropowerPlant> ListFactories()
        {
            var hydropowerPlants = _hydropowerPlantRepository.Find(m => true).ToList();
            if (User.IsInRole(Constant.Role.Member))
            {
                hydropowerPlants = hydropowerPlants.Where(m => m.Owners.Contains(User.GetId())).ToList();
            }

            return hydropowerPlants;
        }
    }
}
