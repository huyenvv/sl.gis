using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IPostDataRepository postDataRepository, IHydropowerPlantRepository hydropowerPlantRepository, IElementRepository elementRepository)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _elementRepository = elementRepository;
        }

        public PagerViewModel ViewModel { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet(Guid? hydropowerPlantId, DateTime? startDate, DateTime? endDate, int? pageIndex = 1)
        {
            var hydropowerPlants = ListFactories();
            if (hydropowerPlants.Count == 0 || hydropowerPlantId.HasValue && !hydropowerPlants.Any(m => m.Id == hydropowerPlantId))
            {
                return NotFound();
            }

            hydropowerPlantId ??= hydropowerPlants.First().Id;
            ViewData["HydropowerPlantId"] = hydropowerPlantId;

            Expression<Func<Core.PostData, bool>> predicate = m => m.HydropowerPlantId == hydropowerPlantId;
            if (startDate.HasValue)
            {
                predicate = m => m.Created >= startDate;
            }
            if (endDate.HasValue)
            {
                predicate = m => m.Created <= endDate;
            }

            var postDatas = _postDataRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

            var pager = new Pager(postDatas.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", new { hydropowerPlantId, startDate, endDate }),
                Items = postDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };

            Elements = _elementRepository.Find(m => true).OrderBy(m => m.Id).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _postDataRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted postData {id}");

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
