using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Notify
{
    [Authorize]
    public class DetailModel : PageModelBase
    {
        private readonly ILogger<DetailModel> _logger;
        private readonly INotifyRepository _notifyRepository;
        private readonly IFileService _fileService;

        public DetailModel(ILogger<DetailModel> logger, INotifyRepository notifyRepository, IFileService fileService, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _notifyRepository = notifyRepository;
            _fileService = fileService;
        }

        public Core.Notify Notify { get; set; }

        public async Task<IActionResult> OnGet(Guid id)
        {
            Notify = await _notifyRepository.GetAsync(m => m.Id == id);
            if (User.Identity.IsAuthenticated)
            {
                if (!Notify.ReadUserIds.Any(m => m == User.GetId().ToString()))
                    await _notifyRepository.SetReadAsync(id, User.GetId().ToString());
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadFileAsync(Guid id, string fileName)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var notify = await _notifyRepository.GetAsync(id);
            if (notify == null)
            {
                return NotFound();
            }

            var filePath = notify.Files.FirstOrDefault(m => Path.GetFileName(m) == fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(filePath))
                return Content("filename not present");

            var memory = await _fileService.GetAsync(filePath);
            return File(memory, Common.GetContentType(filePath), Path.GetFileName(filePath));
        }
    }
}
