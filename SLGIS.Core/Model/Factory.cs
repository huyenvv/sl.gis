using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    public class Factory : BaseEntity
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Title { get; set; }

        public string SendKey { get; set; }

        public string Owner { get; set; }
    }
}
