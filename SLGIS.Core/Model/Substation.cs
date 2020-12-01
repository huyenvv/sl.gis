using MongoDB.Bson.Serialization.Attributes;
using System;
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
    }
}
