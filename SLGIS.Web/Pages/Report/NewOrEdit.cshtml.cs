using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Report
{
    public class NewOrEditModel : PageModel
    {
        private readonly IReportRepository _reportRepository;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IFileService _fileService;

        public NewOrEditModel(IReportRepository reportRepository, IHydropowerPlantRepository hydropowerPlantRepository, IFileService fileService)
        {
            _reportRepository = reportRepository;
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public Core.Report Report { get; set; }

        [BindProperty]
        public List<IFormFile> Files { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? hydropowerPlantId, Guid? id)
        {
            var hydropowerPlants = ListFactories();

            if (hydropowerPlants.Count == 0 || hydropowerPlantId.HasValue && !hydropowerPlants.Any(m => m.Id == hydropowerPlantId))
            {
                return NotFound();
            }

            CreateViewData(hydropowerPlantId);

            if (id == null)
            {
                Report = new Core.Report();
                return Page();
            }

            Report = await _reportRepository.GetAsync(m => m.Id == id);
            if (Report == null || Report.HydropowerPlantId != hydropowerPlantId)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CreateViewData(Report.HydropowerPlantId);
                return Page();
            }

            var hydropowerPlants = ListFactories();
            if (!hydropowerPlants.Any(m => m.Id == Report.HydropowerPlantId))
            {
                return NotFound();
            }

            if (Files?.Count() > 0)
            {
                var listFiles = new List<string>();
                foreach (var file in Files)
                {
                    var filePath = await _fileService.UpsertAsync(await file.GetBytes(), file.FileName, null, true);
                    listFiles.Add(filePath);
                }

                Report.Files = listFiles;
            }

            await _reportRepository.UpsertAsync(Report);

            return RedirectToPage("./Index");
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
        }
    }
}
