using System;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Thông số
    /// </summary>
    public class Setting : BaseEntity
    {
        /// <summary>
        /// Separate graph to display in multiple columns
        /// </summary>
        public string SeparatorColumn { get; set; }
    }
}
