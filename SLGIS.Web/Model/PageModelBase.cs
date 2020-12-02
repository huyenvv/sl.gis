using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;

namespace SLGIS.Web
{
    public class PageModelBase : PageModel
    {
        private readonly HydropowerService _hydropowerService;
        public PageModelBase(HydropowerService hydropowerService)
        {
            _hydropowerService = hydropowerService;
        }

        public bool HasHydropower
        {
            get
            {
                return GetCurrentHydropower() != null;
            }
        }

        public IActionResult OnGetHydropowerChange(Guid id)
        {
            _hydropowerService.ChangeCurrent(User.GetId(), id);
            return LocalRedirect(Request.Path);
        }

        public HydropowerPlant GetCurrentHydropower()
        {
            return _hydropowerService.GetCurrent(User.GetId());
        }

        public IEnumerable<(Guid Id, string Name)> ListCurrentHydropowers()
        {
            return _hydropowerService.CurrentList(User.GetId());
        }

        public IActionResult ReturnToMap()
        {
            return RedirectToPage("/Map/Index");
        }

        public IActionResult ReturnToError()
        {
            return RedirectToPage("/Error");
        }
    }
}
