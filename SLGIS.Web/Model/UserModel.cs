using System.ComponentModel.DataAnnotations;

namespace SLGIS.Web
{
    public class UserModel
    {
        public string Id { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Name { get; set; }

        [Display(Name = "Tài khoản (định dạng email)")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Admin nhóm?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu chưa khớp")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Khóa tài khoản?")]
        public bool IsLocked { get; set; } = false;

        [Display(Name = "Đặt lại mật khẩu?")]
        public bool IsChangePassword { get; set; }
    }
}
