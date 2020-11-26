using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SLGIS.Core.Model
{
    /// <summary>
    /// Nhà máy thủy điện
    /// </summary>
    [BsonIgnoreExtraElements]
    public class HydropowerPlant : BaseEntity
    {
        /// <summary>
        /// Chủ đầu tư
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Người đại diện
        /// </summary>
        public string PersonOwnerName { get; set; }
        /// <summary>
        /// Số điện thoại (của người đại diện)
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email (của người đại diện)
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Tên công trình
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Công suất lắp máy
        /// </summary>
        public string Wattage { get; set; }
        /// <summary>
        /// Cấp công trình
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// Địa điểm xây dựng
        /// </summary>
        public string BuildAddress { get; set; }
        /// <summary>
        /// Diện tích đất
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Năm khởi công xây dựng
        /// </summary>
        public int StartBuild { get; set; }
        /// <summary>
        /// Năm hoàn thành
        /// </summary>
        public int EndBuild { get; set; }
        /// <summary>
        /// Tổng mức đầu tư
        /// </summary>
        public decimal TotalInvestment { get; set; }

        /// <summary>
        /// Tọa độ công trình
        /// </summary>
        public (string lat, string lng) Location { get; set; }
        /// <summary>
        /// Những người có quyền Owner với record này
        /// </summary>
        public List<Guid> Owners { get; set; } = new List<Guid>();

        /// <summary>
        /// Liên kết với bảng HydropowerDams
        /// </summary>
        public Guid HydropowerDamsId { get; set; }

        public string Imange { get; set; }
    }
}
