using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SLGIS.Core;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Element
{
    public class NewOrEditModel : PageModel
    {
        private readonly IElementRepository _elementRepository;

        public NewOrEditModel(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }

        [BindProperty]
        public Core.Element Element { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                Element = new Core.Element();
                return Page();
            }

            Element = await _elementRepository.GetAsync(m => m.Id == id);

            if (Element == null)
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

            await _elementRepository.UpsertAsync(Element);

            return RedirectToPage("./Index");
        }
    }
}
