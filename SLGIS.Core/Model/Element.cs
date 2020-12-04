using System;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Thông số
    /// </summary>
    public class Element : BaseEntity
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Title { get; set; }

        public string Unit { get; set; }
        public string Code { get; set; }

        public Guid? HydropowerPlantId { get; set; }
    }
}
