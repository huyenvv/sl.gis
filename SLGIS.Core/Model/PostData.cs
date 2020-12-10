using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLGIS.Core
{
    /// <summary>
    /// Dữ liệu trung bình ngày
    /// </summary>
    public class PostData : BaseEntity
    {
        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sản lượng ngày (MWh)
        /// </summary>
        public double SanLuongNgay { get; set; }

        /// <summary>
        /// Tổng lượng nước qua Tuabin (m3)
        /// </summary>
        public double TotalWater { get; set; }

        /// <summary>
        /// Số giờ phát điện (giờ)
        /// </summary>
        public double SoGioPhatDien { get; set; }

        public Guid HydropowerPlantId { get; set; }

        public List<PostDataDetails> PostDataDetails { get; set; } = new List<PostDataDetails>();

        public bool CanEdit()
        {
            var secondsOfDay = 2 * 60 * 60;
            return (DateTime.UtcNow.AddHours(7) - Created.ToUniversalTime()).TotalSeconds < secondsOfDay;
        }
    }
}
