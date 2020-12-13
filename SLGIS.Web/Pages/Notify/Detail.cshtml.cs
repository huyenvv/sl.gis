using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
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
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        public DetailModel(ILogger<DetailModel> logger, INotifyRepository notifyRepository, IUserRepository userRepository, IFileService fileService, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _notifyRepository = notifyRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        public Core.Notify Notify { get; set; }
        public Dictionary<string, string> Users { get; set; } = new Dictionary<string, string>();
        public Dictionary<Guid, string> Plants { get; set; } = new Dictionary<Guid, string>();

        public async Task<IActionResult> OnGet(Guid id)
        {
            Notify = await _notifyRepository.GetAsync(m => m.Id == id);
            if (CanManage)
            {
                Users = _userRepository.Find(m => m.Roles.Count == 0).ToDictionary(m => m.Id.ToString(), m => m.UserName);
                Plants = ListCurrentHydropowers().ToDictionary(m => m.Id, m => m.Name);
            }
            else
            {
                var plantId = GetCurrentHydropower().Id;
                if (!Notify.IsRead(plantId, User.GetId()))
                    await _notifyRepository.SetReadAsync(id, plantId, User.GetId().ToString());
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
