using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Factory
{
    public class NewOrEditModel : PageModel
    {
        private readonly IFactoryRepository _factoryRepository;

        public NewOrEditModel(IFactoryRepository factoryRepository)
        {
            _factoryRepository = factoryRepository;
        }

        [BindProperty]
        public Core.Factory Factory { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                Factory = new Core.Factory();
                return Page();
            }

            Factory = await _factoryRepository.GetAsync(m => m.Id == id);

            if (Factory == null)
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

            await _factoryRepository.UpsertAsync(Factory);

            return RedirectToPage("./Index");
        }
    }
}
