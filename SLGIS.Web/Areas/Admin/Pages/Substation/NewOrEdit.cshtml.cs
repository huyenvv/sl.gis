using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Substation
{
    public class NewOrEditModel : PageModel
    {
        private readonly ISubstationRepository _substationRepository;

        public NewOrEditModel(ISubstationRepository substationRepository)
        {
            _substationRepository = substationRepository;
        }

        [BindProperty]
        public Core.Model.Substation Substation { get; set; }

        [BindProperty]
        public string Location { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                Substation = new Core.Model.Substation();
                return Page();
            }

            Substation = await _substationRepository.GetAsync(m => m.Id == id);

            if (Substation == null)
            {
                return NotFound();
            }
            Location = Substation.Location.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Location != null && Location.Split(',').Length >= 2)
            {
                Substation.Location.Lat = Location.Split(',')[0].Trim();
                Substation.Location.Lng = Location.Split(',')[1].Trim();
            }
            await _substationRepository.UpsertAsync(Substation);

            return RedirectToPage("./Index");
        }
    }
}
