using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Báo cáo tài liệu của các thủy điện
    /// </summary>
    public class Report : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public List<string> Files { get; set; } = new List<string>();

        [Required]
        public Guid HydropowerPlantId { get; set; }

        public bool CanEdit()
        {
            var secondsOf2Hours = 2 * 60 * 60;
            return (DateTime.UtcNow.AddHours(7) - Created.ToUniversalTime()).TotalSeconds < secondsOf2Hours;
        }
    }
}
