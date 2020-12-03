using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace SLGIS.Core.Model
{
    /// <summary>
    /// Trạm biến áp/ cột
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Substation : BaseEntity
    {
        /// <summary>
        /// Tên trạm biến áp
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tọa độ
        /// </summary>
        public Location Location { get; set; } = new Location();

        /// <summary>
        /// Vị trí cột
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Tên đường dây
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// Cấp điện áp
        /// </summary>
        public string ElectricLevel { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        [BsonIgnore]

        public static List<string> ElectricLevels = new List<string> { "500KV", "220KV", "110KV", "35KV" };
    }
}
