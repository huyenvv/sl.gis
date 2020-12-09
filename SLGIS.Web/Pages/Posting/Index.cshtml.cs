using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IPostDataRepository postDataRepository, IElementRepository elementRepository, HydropowerService hydropowerService)
            : base(hydropowerService)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public SearchModel SearchModel { get; set; } = new SearchModel();
        public PagerViewModel ViewModel { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet(SearchModel searchModel, int? pageIndex = 1)
        {
            if (!HasHydropower)
            {
                return ReturnToHydropower();
            }

            if (searchModel != null)
                SearchModel = searchModel;

            var hydropowerPlantId = GetCurrentHydropower().Id;
            ViewData["HydropowerPlantId"] = hydropowerPlantId;

            var list = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).AsQueryable();
            if (SearchModel.StartDate.HasValue)
            {
                list = list.Where(m => m.Date >= SearchModel.StartDate);
            }

            if (SearchModel.EndDate.HasValue)
            {
                SearchModel.EndDate = SearchModel.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                list = list.Where(m => m.Date <= SearchModel.EndDate);
            }

            var postDatas = list.OrderByDescending(m => m.Date).AsEnumerable();
            var pager = new Pager(postDatas.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", new { hydropowerPlantId, SearchModel.StartDate, SearchModel.EndDate }),
                Items = postDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };

            Elements = _elementRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).OrderBy(m => m.Id).ToList();
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
    }
}
