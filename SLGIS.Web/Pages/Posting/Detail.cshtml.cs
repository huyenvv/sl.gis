using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.PostData
{
    [Authorize]
    public class DetailModel : PageModelBase
    {
        private readonly ILogger<DetailModel> _logger;
        private readonly IPostDataRepository _postDataRepository;
        private readonly IElementRepository _elementRepository;

        public DetailModel(ILogger<DetailModel> logger, IPostDataRepository postDataRepository, HydropowerService hydropowerService, IElementRepository elementRepository)
             : base(hydropowerService)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.PostData PostData { get; set; }
        public List<Core.Element> Elements { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (!HasHydropower)
            {
                return ReturnToMap();
            }


            PostData = await _postDataRepository.GetAsync(m => m.Id == id);// && m.HydropowerPlantId == hydropowerPlantId);
            if (PostData == null)
            {
                return NotFound();
            }

            if (CanManage)
            {
                Elements = _elementRepository.Find(m => PostData.HydropowerPlantId == m.HydropowerPlantId).ToList();
                if (Elements.Count == 0)
                {
                    Elements = _elementRepository.Find(m => !m.HydropowerPlantId.HasValue).ToList();
                }
            }
            else
            {
                var hydropowerPlantId = GetCurrentHydropower().Id;
                if (PostData.HydropowerPlantId != hydropowerPlantId)
                {
                    return NotFound();
                }

                Elements = _elementRepository.Find(m => hydropowerPlantId == m.HydropowerPlantId).ToList();
            }


            if (PostData.PostDataDetails?.Count < 1)
            {
                PostData.PostDataDetails = new List<PostDataDetails> { new PostDataDetails() };
            }


            return Page();
        }
    }
}
