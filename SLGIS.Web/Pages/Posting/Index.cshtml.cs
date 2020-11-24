using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize(Constant.Role.Member)]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IFactoryRepository _factoryRepository;

        public IndexModel(ILogger<IndexModel> logger, IPostDataRepository postDataRepository, IFactoryRepository factoryRepository)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _factoryRepository = factoryRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public async Task<IActionResult> OnGet(Guid? factoryId, DateTime? startDate, DateTime? endDate, int? pageIndex = 1)
        {
            var factories = await _factoryRepository.Find(m => m.Owner == User.Identity.Name).ToListAsync();
            if (factories.Count == 0 || factoryId.HasValue && !factories.Any(m => m.Id == factoryId))
            {
                return NotFound();
            }

            factoryId ??= factories.First().Id;

            Expression<Func<Core.PostData, bool>> predicate = m => m.FactoryId == factoryId;
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
                BaseUrl = Url.Page("./Index"),
                Items = postDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
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

            await _postDataRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted postData {id}");

            return RedirectToPage("./Index");
        }
    }
}
