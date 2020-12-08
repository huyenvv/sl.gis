using System.ComponentModel.DataAnnotations;

namespace SLGIS.Web
{
    public class RoleRightModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        [Display(Name = "Quản trị viên?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Chuyên viên?")]
        public bool IsMember { get; set; } = false;
    }
}
