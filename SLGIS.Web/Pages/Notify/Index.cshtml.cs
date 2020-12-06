using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
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

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public IActionResult OnGet(DateTime? startDate, DateTime? endDate, string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            var list = _notifyRepository.Find(m => true).AsQueryable();
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
