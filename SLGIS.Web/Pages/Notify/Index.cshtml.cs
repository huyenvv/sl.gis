using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Extension;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using SLGIS.Web.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Notify
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INotifyRepository _notifyRepository;
        private readonly IFileService _fileService;

        public IndexModel(ILogger<IndexModel> logger, INotifyRepository notifyRepository, IFileService fileService, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _notifyRepository = notifyRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public SearchModel SearchModel { get; set; } = new SearchModel();
        public PagerViewModel ViewModel { get; set; }

        public IActionResult OnGet(SearchModel searchModel, int? pageIndex = 1)
        {
            if (!HasHydropower)
            {
                return ReturnToHydropower();
            }

            if (searchModel != null)
                SearchModel = searchModel;

            var list = _notifyRepository.Find(m => true).AsQueryable();
            if (!CanManage)
            {
                list = list.Where(m => m.IsAll || m.ToPlantIds.Any(n => n == GetCurrentHydropower().Id));
            }
            if (!string.IsNullOrEmpty(SearchModel.FilterText))
            {
                list = list.Where(m => m.Title.Contains(SearchModel.FilterText.ToLower()));
            }

            if (SearchModel.StartDate.HasValue)
            {
                var startDate = SearchModel.StartDate.Value.Date.ToVNDate();
                list = list.Where(m => m.Created >= startDate);
            }

            if (SearchModel.EndDate.HasValue)
            {
                var endDate = SearchModel.EndDate.Value.Date.ToVNDate().AddDays(1).AddSeconds(-1);
                list = list.Where(m => m.Created <= endDate);
            }

            var pager = new Pager(list.Count(), pageIndex);
            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", searchModel.ToPagingModel()),
                Items = list.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).OrderByDescending(m => m.Created).ToList(),
                Pager = pager
            };

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty || !CanManage)
            {
                return NotFound();
            }

            var notify = await _notifyRepository.GetAsync(m => m.Id == id);
            foreach (var path in notify.Files)
            {
                await _fileService.DeleteAsync(path);
            }

            await _notifyRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted notify {id}");

            return RedirectToPage("./Index");
        }
    }
}
