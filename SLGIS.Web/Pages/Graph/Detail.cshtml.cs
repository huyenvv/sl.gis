using Microsoft.AspNetCore.Authorization;
using SLGIS.Implementation;

namespace SLGIS.Web.Pages.Detail
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        public IndexModel(HydropowerService hydropowerService) : base(hydropowerService)
        {
        }

        public void OnGet()
        {
        }
    }
}
