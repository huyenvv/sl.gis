using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _substationRepository.UpsertAsync(Substation);

            return RedirectToPage("./Index");
        }
    }
}
