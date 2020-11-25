using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize]
    public class NewOrEditModel : PageModel
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IFactoryRepository _factoryRepository;
        private readonly IElementRepository _elementRepository;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IPostDataRepository postDataRepository, IFactoryRepository factoryRepository, IElementRepository elementRepository)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _factoryRepository = factoryRepository;
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.PostData PostData { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet(Guid? factoryId)
        {
            var factories = ListFactories();

            if (factories.Count == 0 || factoryId.HasValue && !factories.Any(m => m.Id == factoryId))
            {
                return NotFound();
            }

            CreateViewData(factoryId);

            PostData = new Core.PostData
            {
                Time = DateTime.Now,
                FactoryId = factoryId.Value
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CreateViewData(PostData.FactoryId);
                return Page();
            }

            var factories = ListFactories();
            if (!factories.Any(m => m.Id == PostData.FactoryId))
            {
                return NotFound();
            }

            await _postDataRepository.AddAsync(PostData);

            _logger.LogInformation($"Add postData {PostData.Id}");

            return RedirectToPage("./Index", new { PostData.FactoryId });
        }

        private List<Core.Factory> ListFactories()
        {
            if (Factories == null || Factories.Count == 0)
            {
                Factories = _factoryRepository.Find(m => true).ToList();
            }

            var factories = Factories;
            if (User.IsInRole(Constant.Role.Member))
            {
                factories = factories.Where(m => m.Owner == User.Identity.Name).ToList();
            }

            return factories;
        }


        private List<Core.Factory> Factories { get; set; }
        private void CreateViewData(Guid? factoryId)
        {
            var factories = ListFactories();
            ViewData["FactoryId"] = factoryId ?? factories.First().Id;
            Elements = _elementRepository.Find(m => true).OrderBy(m => m.Id).ToList();
        }
    }
}
