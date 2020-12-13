using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Extension;
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
        public Core.PostData Total { get; set; } = new Core.PostData();
        public SelectList HydropowerSelectList { get; set; }

        public IActionResult OnGetAsync(SearchModel searchModel, Guid? hydropowerId = null, int? pageIndex = 1)
        {
            if (!HasHydropower)
            {
                return ReturnToHydropower();
            }

            if (searchModel != null)
                SearchModel = searchModel;

            if (!CanManage)
            {
                hydropowerId = GetCurrentHydropower().Id;
            }

            HydropowerSelectList = CreateHydropowerPlantSelection(hydropowerId);

            var list = _postDataRepository.Find(m => true).AsQueryable();
            if (hydropowerId.HasValue)
            {
                list = list.Where(m => m.HydropowerPlantId == hydropowerId);
            }

            if (SearchModel.StartDate.HasValue)
            {
                var startDate = SearchModel.StartDate.Value.Date.ToVNDate();
                list = list.Where(m => m.Date >= startDate);
            }

            if (SearchModel.EndDate.HasValue)
            {
                var endDate = SearchModel.EndDate.Value.Date.ToVNDate().AddDays(1).AddSeconds(-1);
                list = list.Where(m => m.Date <= endDate);
            }

            var postDatas = list.OrderByDescending(m => m.HydropowerPlantId).ThenByDescending(m => m.Date).AsEnumerable();
            Total.TotalWater = postDatas.Sum(m => m.TotalWater);
            Total.SanLuongNgay = postDatas.Sum(m => m.SanLuongNgay);
            Total.SoGioPhatDien = postDatas.Sum(m => m.SoGioPhatDien);

            var pager = new Pager(postDatas.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", new { hydropowerId, SearchModel.StartDate, SearchModel.EndDate }),
                Items = postDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };

            Elements = _elementRepository.Find(m => m.HydropowerPlantId == hydropowerId).OrderBy(m => m.Id).ToList();
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
