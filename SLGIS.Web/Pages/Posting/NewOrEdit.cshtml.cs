//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
//using SLGIS.Core;
//using System;
//using System.Threading.Tasks;

//namespace SLGIS.Web.Pages.PostData
//{
//    public class NewOrEditModel : PageModel
//    {
//        private readonly ILogger<NewOrEditModel> _logger;
//        private readonly IPostDataRepository _postDataRepository;

//        public NewOrEditModel(ILogger<NewOrEditModel> logger, IPostDataRepository postDataRepository)
//        {
//            _logger = logger;
//            _postDataRepository = postDataRepository;
//        }

//        [BindProperty]
//        public SLGIS.Core.PostData PostData { get; set; }

//        public async Task OnGet(Guid? id = null)
//        {
//            PostData = new SLGIS.Core.PostData();

//            if (id.HasValue && id != Guid.Empty)
//            {
//                PostData = await _postDataRepository.GetAsync(id.Value);
//            }
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

//            var existed = await _postDataRepository.GetAsync(m => m.IpAddress == PostData.IpAddress && m.Id != PostData.Id);

//            if (existed != null)
//            {
//                ModelState.AddModelError("", "Địa chỉ IP đã tồn tại. Vui lòng kiểm tra lại.");
//                return Page();
//            }

//            PostData.Updated = DateTime.Now;
//            PostData.UpdatedBy = User.Identity.Name;
//            PostData.IpAddress = PostData.IpAddress.Trim();

//            await _postDataRepository.UpsertAsync(PostData);

//            _logger.LogInformation($"Updated postData {PostData.Id}");

//            return RedirectToPage("/PostData/Index");
//        }
//    }
//}
