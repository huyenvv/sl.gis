using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Dữ liệu các nhà máy gửi lên
    /// </summary>
    public class PostData : BaseEntity
    {
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public IEnumerable<ElementValue> Values { get; set; }

        [Required]
        public Guid FactoryId { get; set; }

        public string Note { get; set; }
    }

    public class ElementValue
    {
        [Required]
        public double Value { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
