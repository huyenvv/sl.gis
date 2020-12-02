using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using SLGIS.Implementation;

namespace SLGIS.Web
{
    public class PageModelBase : PageModel
    {
        private readonly HydropowerService _hydropowerService;
        public PageModelBase(HydropowerService hydropowerService)
        {
            _hydropowerService = hydropowerService;
        }

        public HydropowerPlant GetCurrentHydropower()
        {
            return _hydropowerService.GetCurrent(User.GetId());
        }
    }
}
