using System.ComponentModel.DataAnnotations;

namespace SLGIS.Web
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        //[EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
