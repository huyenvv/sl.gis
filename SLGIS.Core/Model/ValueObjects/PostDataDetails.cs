using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Dữ liệu các nhà máy gửi lên
    /// </summary>
    public class PostDataDetails
    {
        [Required]
        public int Hour { get; set; }
        public DateTime Time { get; set; }

        [Required]
        public IList<ElementValue> Values { get; set; } = new List<ElementValue>();
    }

    public class ElementValue
    {
        [Required]
        public double Value { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
