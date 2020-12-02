using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Report
{
    public class NewOrEditModel : PageModelBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IFileService _fileService;

        public NewOrEditModel(IReportRepository reportRepository, HydropowerService hydropowerService, IFileService fileService) : base(hydropowerService)
        {
            _reportRepository = reportRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public Core.Report Report { get; set; }

        [BindProperty]
        public List<IFormFile> Files { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            var currentHydropower = GetCurrentHydropower();
            if (currentHydropower == null)
            {
                return RedirectToPage("/Map/Index");
            }

            
            ViewData["HydropowerPlantId"] = currentHydropower.Id;

            if (id == null)
            {
                Report = new Core.Report();
                return Page();
            }

            Report = await _reportRepository.GetAsync(m => m.Id == id);
            if (Report == null || Report.HydropowerPlantId != currentHydropower.Id)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["HydropowerPlantId"] = Report.HydropowerPlantId;
                return Page();
            }

            if (Report.HydropowerPlantId != GetCurrentHydropower().Id)
            {
                return BadRequest();
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
    }
}
