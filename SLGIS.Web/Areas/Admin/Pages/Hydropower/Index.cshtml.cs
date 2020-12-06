using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Hydropower
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IHydropowerPlantRepository hydropowerPlantRepository, HydropowerService hydropowerService
            , IElementRepository elementRepository)
            : base(hydropowerService)
        {
            _logger = logger;
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _elementRepository = elementRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            var plants = _hydropowerPlantRepository.Find(m => true).AsQueryable();
            if (!string.IsNullOrEmpty(FilterText))
            {
                plants = plants.Where(m => m.Name.ToLower().Contains(FilterText.ToLower()));
            }

            if (!CanManage)
            {
                plants = plants.Where(m => m.Owners.Contains(User.GetId()));
            }

            var data = plants.OrderByDescending(m => m.Created).ToList();

            var pager = new Pager(data.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
                Items = data.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            var plant = await _hydropowerPlantRepository.GetAsync(id);
            if (!CanManage && !plant.Owners.Any(m => m == User.GetId()))
            {
                return BadRequest();
            }

            await _hydropowerPlantRepository.DeleteAsync(id);
            var elements = _elementRepository.Find(m => m.HydropowerPlantId == plant.Id).ToList();
            foreach (var item in elements)
            {
                await _elementRepository.DeleteAsync(item.Id);
            }
            _logger.LogInformation($"Deleted hydropowerPlant {id}");

            return RedirectToPage("./Index");
        }
    }
}
