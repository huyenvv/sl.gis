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
        private readonly IFactoryRepository _factoryRepository;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IPostDataRepository postDataRepository, IFactoryRepository factoryRepository, IElementRepository elementRepository)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _factoryRepository = factoryRepository;
            _elementRepository = elementRepository;
        }

        public PagerViewModel ViewModel { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet(Guid? factoryId, DateTime? startDate, DateTime? endDate, int? pageIndex = 1)
        {
            var factories = ListFactories();
            if (factories.Count == 0 || factoryId.HasValue && !factories.Any(m => m.Id == factoryId))
            {
                return NotFound();
            }

            factoryId ??= factories.First().Id;
            ViewData["FactoryId"] = factoryId;

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
                BaseUrl = Url.Page("./Index", new { factoryId, startDate, endDate }),
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

        private List<Core.Factory> ListFactories()
        {
            var factories = _factoryRepository.Find(m => true).ToList();
            if (User.IsInRole(Constant.Role.Member))
            {
                factories = factories.Where(m => m.Owner == User.Identity.Name).ToList();
            }

            return factories;
        }
    }
}
