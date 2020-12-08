using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Thông báo của sở cho các thủy điện
    /// </summary>
    public class Notify : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }

        public List<string> Files { get; set; } = new List<string>();

        public bool CanEdit()
        {
            var secondsOfDay = 24 * 60 * 60;
            return (DateTime.UtcNow.AddHours(7) - Created.ToUniversalTime()).TotalSeconds < secondsOfDay;
        }
    }
}
