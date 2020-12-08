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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!HasHydropower)
            {
                return ReturnToMap();
            }

            var hydropowerPlantId = GetCurrentHydropower().Id;
            CreateViewData(hydropowerPlantId);

            if (id != null)
                PostData = await _postDataRepository.GetAsync(id.Value);
            if (PostData == null || PostData.PostDataDetails?.Count < 1)
            {
                PostData = new Core.PostData
                {
                    Date = DateTime.Now.Date,
                    PostDataDetails = new List<PostDataDetails> { new PostDataDetails() }
                };
            }

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
            var details = new List<PostDataDetails>();
            foreach (var item in PostData.PostDataDetails)
            {
                var sum = item.Values.Sum(m => m.Value);
                if (sum != 0) details.Add(item);
            }
            PostData.PostDataDetails = details;

            PostData.Date = PostData.Date.AddHours(7);
            await _postDataRepository.UpsertAsync(PostData);

            _logger.LogInformation($"Add postData {PostData.Id}");

            return RedirectToPage("./Index", new { PostData.HydropowerPlantId });
        }

        private void CreateViewData(Guid? hydropowerPlantId)
        {
            ViewData["HydropowerPlantId"] = hydropowerPlantId;
            Elements = _elementRepository.Find(m => hydropowerPlantId == m.HydropowerPlantId).ToList();
        }
    }
}
