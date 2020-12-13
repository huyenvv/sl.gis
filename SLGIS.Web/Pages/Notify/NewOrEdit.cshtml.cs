using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Notify
{
    [Authorize]
    public class NewOrEditModel : PageModelBase
    {
        private readonly INotifyRepository _notifyRepository;
        private readonly IFileService _fileService;

        public NewOrEditModel(INotifyRepository notifyRepository, HydropowerService hydropowerService, IFileService fileService) : base(hydropowerService)
        {
            _notifyRepository = notifyRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public Core.Notify Notify { get; set; }
        public IEnumerable<(Guid Id, string Name)> Plants { get; set; }

        [BindProperty]
        public List<IFormFile> Files { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!CanManage)
            {
                return RedirectToPage("./Index");
            }

            Plants = ListCurrentHydropowers();

            if (id == null)
            {
                Notify = new Core.Notify();
                return Page();
            }

            Notify = await _notifyRepository.GetAsync(m => m.Id == id);
            if (!Notify.CanEdit())
            {
                return RedirectToPage("./Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Files?.Count() > 0)
            {
                var listFiles = new List<string>();
                foreach (var file in Files)
                {
                    var filePath = await _fileService.UpsertAsync(await file.GetBytes(), file.FileName, null, true);
                    listFiles.Add(filePath);
                }

                Notify.Files = listFiles;
            }
            else
            {
                if (Notify.Id != Guid.Empty)
                {
                    var report = await _notifyRepository.GetAsync(Notify.Id);
                    Notify.Files = report.Files;
                }
            }

            if (Notify.ToPlantIds.Count == 0)
            {
                Notify.IsAll = true;
            }
            await _notifyRepository.UpsertAsync(Notify);

            return RedirectToPage("./Index");
        }
    }
}
