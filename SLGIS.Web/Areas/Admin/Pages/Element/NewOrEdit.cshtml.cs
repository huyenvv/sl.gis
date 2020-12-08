using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Element
{
    [Authorize]
    public class NewOrEditModel : PageModelBase
    {
        private readonly IElementRepository _elementRepository;

        public NewOrEditModel(IElementRepository elementRepository, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.Element Element { get; set; }
        public SelectList HydropowerSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            CreateHydropowerSelection();
            if (id == null)
            {
                Element = new Core.Element();
                return Page();
            }

            Element = await _elementRepository.GetAsync(m => m.Id == id);

            if (Element == null || !CanManage && GetCurrentHydropower().Id != Element.HydropowerPlantId)
            {
                return NotFound();
            }

            CreateHydropowerSelection(Element.HydropowerPlantId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CreateHydropowerSelection(Element.HydropowerPlantId);
                return Page();
            }

            if (!CanManage)
            {
                if (Element.HydropowerPlantId != null && Element.HydropowerPlantId != Guid.Empty)
                {
                    return BadRequest();
                }

                Element.HydropowerPlantId = GetCurrentHydropower().Id;
            }

            await _elementRepository.UpsertAsync(Element);
            return RedirectToPage("./Index");
        }

        private void CreateHydropowerSelection(object selected = null)
        {
            HydropowerSelectList = CreateHydropowerPlantSelection(selected);
        }
    }
}
