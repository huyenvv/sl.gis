using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLGIS.Web
{
    public class PageModelBase : PageModel
    {
        private readonly HydropowerService _hydropowerService;
        public PageModelBase(HydropowerService hydropowerService)
        {
            _hydropowerService = hydropowerService;
        }

        public bool CanManage
        {
            get
            {
                return Constant.Role.All.Any(User.IsInRole);
            }
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

        public IActionResult ReturnToHydropower()
        {
            return RedirectToPage("/Hydropower/Index", new { area = "Admin" });
        }

        public IActionResult ReturnToError()
        {
            return RedirectToPage("/Error");
        }

        public SelectList CreateHydropowerPlantSelection(object selected = null)
        {
            var list = ListCurrentHydropowers().ToDictionary(m => m.Id, m => m.Name);
            var selectList = new SelectList(list, "Key", "Value");
            if (selected != null)
            {
                selectList = new SelectList(list, "Key", "Value", selected);
            }

            return selectList;
        }
    }
}
