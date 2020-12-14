using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Implementation;

namespace SLGIS.Web.Pages.Detail
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ISettingRepository _settingRepository;
        public IndexModel(ISettingRepository settingRepository, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _settingRepository = settingRepository;
        }

        public void OnGet()
        {
            var setting = _settingRepository.Find(m => true).FirstOrDefault();
            ViewData["GraphSeparator"] = setting?.SeparatorColumn;
        }
    }
}
