using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SLGIS.Core
{
    public class Computer : BaseEntity
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Title { get; set; }

        [Display(Name = "Địa chỉ IP")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string IpAddress { get; set; }

        [JsonIgnore]
        public string UpdatedBy { get; set; }

        public DateTime Updated { get; set; }
    }
}
