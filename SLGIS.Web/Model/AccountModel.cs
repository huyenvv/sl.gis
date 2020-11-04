using System.ComponentModel.DataAnnotations;

namespace SLGIS.Web
{
    public class AccountModel
    {
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Name { get; set; }

        [Display(Name = "Tài khoản")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu cũ")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Nhập lại mật khẩu chưa khớp")]
        public string ConfirmNewPassword { get; set; }

        [Display(Name = "Đặt lại mật khẩu?")]
        public bool IsChangePassword { get; set; }
    }
}
