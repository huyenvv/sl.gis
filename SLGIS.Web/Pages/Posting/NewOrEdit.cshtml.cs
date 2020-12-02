using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize]
    public class NewOrEditModel : PageModelBase
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IElementRepository _elementRepository;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IPostDataRepository postDataRepository, HydropowerService hydropowerService, IElementRepository elementRepository)
             : base(hydropowerService)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.PostData PostData { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet()
        {
            if (GetCurrentHydropower() == null)
            {
                return RedirectToPage("/Map/Index");
            }

            var hydropowerPlantId = GetCurrentHydropower().Id;
            CreateViewData(hydropowerPlantId);

            PostData = new Core.PostData
            {
                Time = DateTime.Now,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CreateViewData(PostData.HydropowerPlantId);
                return Page();
            }

            if (PostData.HydropowerPlantId != GetCurrentHydropower().Id)
            {
                return BadRequest();
            }

            await _postDataRepository.AddAsync(PostData);

            _logger.LogInformation($"Add postData {PostData.Id}");

            return RedirectToPage("./Index", new { PostData.HydropowerPlantId });
        }

        private void CreateViewData(Guid? hydropowerPlantId)
        {
            ViewData["HydropowerPlantId"] = hydropowerPlantId;
            Elements = _elementRepository.Find(m => true).OrderBy(m => m.Id).ToList();
        }
    }
}
