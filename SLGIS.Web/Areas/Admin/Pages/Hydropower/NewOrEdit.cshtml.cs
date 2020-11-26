using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Hydropower
{
    public class NewOrEditModel : PageModel
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;

        public NewOrEditModel(IHydropowerPlantRepository hydropowerPlantRepository)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
        }

        [BindProperty]
        public Core.HydropowerPlant HydropowerPlant { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                HydropowerPlant = new Core.HydropowerPlant();
                return Page();
            }

            HydropowerPlant = await _hydropowerPlantRepository.GetAsync(m => m.Id == id);

            if (HydropowerPlant == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _hydropowerPlantRepository.UpsertAsync(HydropowerPlant);

            return RedirectToPage("./Index");
        }
    }
}
