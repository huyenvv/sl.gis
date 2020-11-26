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
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IElementRepository _elementRepository;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IPostDataRepository postDataRepository, IHydropowerPlantRepository hydropowerPlantRepository, IElementRepository elementRepository)
        {
            _logger = logger;
            _postDataRepository = postDataRepository;
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.PostData PostData { get; set; }
        public List<Core.Element> Elements { get; set; }

        public IActionResult OnGet(Guid? hydropowerPlantId)
        {
            var hydropowerPlants = ListFactories();

            if (hydropowerPlants.Count == 0 || hydropowerPlantId.HasValue && !hydropowerPlants.Any(m => m.Id == hydropowerPlantId))
            {
                return NotFound();
            }

            CreateViewData(hydropowerPlantId);

            PostData = new Core.PostData
            {
                Time = DateTime.Now,
                HydropowerPlantId = hydropowerPlantId.Value
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

            var hydropowerPlants = ListFactories();
            if (!hydropowerPlants.Any(m => m.Id == PostData.HydropowerPlantId))
            {
                return NotFound();
            }

            await _postDataRepository.AddAsync(PostData);

            _logger.LogInformation($"Add postData {PostData.Id}");

            return RedirectToPage("./Index", new { PostData.HydropowerPlantId });
        }

        private List<Core.HydropowerPlant> ListFactories()
        {
            if (Factories == null || Factories.Count == 0)
            {
                Factories = _hydropowerPlantRepository.Find(m => true).ToList();
            }

            var hydropowerPlants = Factories;
            if (User.IsInRole(Constant.Role.Member))
            {
                hydropowerPlants = hydropowerPlants.Where(m => m.Owners.Contains(User.GetId())).ToList();
            }

            return hydropowerPlants;
        }


        private List<Core.HydropowerPlant> Factories { get; set; }
        private void CreateViewData(Guid? hydropowerPlantId)
        {
            var hydropowerPlants = ListFactories();
            ViewData["HydropowerPlantId"] = hydropowerPlantId ?? hydropowerPlants.First().Id;
            Elements = _elementRepository.Find(m => true).OrderBy(m => m.Id).ToList();
        }
    }
}
