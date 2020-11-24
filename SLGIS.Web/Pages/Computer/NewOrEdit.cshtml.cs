using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Computer
{
    public class NewOrEditModel : PageModel
    {
        private readonly ILogger<NewOrEditModel> _logger;
        private readonly IFactoryRepository _computerRepository;

        public NewOrEditModel(ILogger<NewOrEditModel> logger, IFactoryRepository computerRepository)
        {
            _logger = logger;
            _computerRepository = computerRepository;
        }

        [BindProperty]
        public SLGIS.Core.Factory Computer { get; set; }

        public async Task OnGet(Guid? id = null)
        {
            Computer = new SLGIS.Core.Factory();

            if (id.HasValue && id != Guid.Empty)
            {
                Computer = await _computerRepository.GetAsync(id.Value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //var existed = await _computerRepository.GetAsync(m => m.IpAddress == Computer.IpAddress && m.Id != Computer.Id);

            //if (existed != null)
            //{
            //    ModelState.AddModelError("", "Địa chỉ IP đã tồn tại. Vui lòng kiểm tra lại.");
            //    return Page();
            //}

            //Computer.Updated = DateTime.Now;
            //Computer.UpdatedBy = User.Identity.Name;
            //Computer.IpAddress = Computer.IpAddress.Trim();

            //await _computerRepository.UpsertAsync(Computer);

            //_logger.LogInformation($"Updated computer {Computer.Id}");

            return RedirectToPage("/Computer/Index");
        }
    }
}
