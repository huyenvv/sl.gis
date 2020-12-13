using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SLGIS.Core.Model.ValueObjects;
using System.Collections.Generic;

namespace SLGIS.Core
{
    /// <summary>
    /// Nhà máy thủy điện
    /// </summary>
    [BsonIgnoreExtraElements]
    public class HydropowerPlant : BaseEntity
    {
        public List<HydropowerDams> HydropowerDams { get; set; } = new List<HydropowerDams>();

        //public HydropowerDams HydropowerDams { get; set; } = new HydropowerDams();

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
        public Location Location { get; set; } = new Location();
        /// <summary>
        /// Những người có quyền Owner với record này
        /// </summary>
        public List<ObjectId> Owners { get; set; } = new List<ObjectId>();

        /// <summary>
        /// Phương án đấu nối. Trên giao diện sẽ chọn nối với trạm biến áp
        /// </summary>
        public List<Connection> Connections { get; set; } = new List<Connection>();

        public string Image { get; set; }
        public string Status { get; set; }

        public string District { get; set; }

        public static List<string> Districts = new List<string>
        {
            "Quỳnh Nhai"
            ,"Mường La"
            ,"Thuận Châu"
            ,"Phù Yên"
            ,"Bắc Yên"
            ,"Mai Sơn"
            ,"Sông Mã"
            ,"Yên Châu"
            ,"Mộc Châu"
            ,"Sốp Cộp"
            ,"Vân Hồ"
        };
    }
}
