using System;

namespace SLGIS.Core.Model.ValueObjects
{
    /// <summary>
    /// Trạm biến áp/ cột
    /// </summary>
    public class Connection : BaseEntity
    {
        /// <summary>
        /// Tên nhà máy, quy mô TBA, cấp điện áp
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Liên kết với bảng Substation. Trên giao diện cho phép chọn
        /// </summary>
        public Guid SubstationId { get; set; }
    }
}
