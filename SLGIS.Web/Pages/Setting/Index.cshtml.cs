using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Implementation;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Setting
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ISettingRepository _settingRepository;

        public IndexModel(ISettingRepository settingRepository, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _settingRepository = settingRepository;
        }

        [BindProperty]
        public Core.Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!CanManage)
            {
                return ReturnToHydropower();
            }

            Setting = _settingRepository.Find(m => true).FirstOrDefault();
            if (Setting == null)
            {
                Setting = await _settingRepository.AddAsync(new Core.Setting());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!CanManage)
            {
                return BadRequest();
            }

            await _settingRepository.UpdateAsync(Setting);
            return RedirectToPage("./Index");
        }
    }
}
