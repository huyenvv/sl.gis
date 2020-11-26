using System;
using System.Collections.Generic;

namespace SLGIS.Core.Model.ValueObjects
{
    /// <summary>
    /// Đập thủy điện
    /// </summary>
    public class HydropowerDams
    {
        /// <summary>
        /// Chủ đầu tư
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Địa điểm xây dựng
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Tọa độ công trình
        /// </summary>
        public (string lat, string lng) Location { get; set; }
        /// <summary>
        /// Nhiệm vụ chính (Cấp nước. Phát điện. Cắt, giảm lũ. Cấp nước sinh hoạt, Cấp nước cho nông nghiệp) multi select
        /// </summary>
        public List<string> Missions { get; set; } = new List<string>();

        /// <summary>
        /// Năm khởi công xây dựng
        /// </summary>
        public int StartBuild { get; set; }
        /// <summary>
        /// Năm hoàn thành
        /// </summary>
        public int EndBuild { get; set; }

        /// <summary>
        /// Nguồn vốn đầu tư xây dựng
        /// </summary>
        public string NguonVon { get; set; }
        /// <summary>
        /// Tên chủ sở hữu đập
        /// </summary>
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerFax { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerWebsite { get; set; }
        /// <summary>
        /// Những người có quyền Owner với record này
        /// </summary>
        public List<Guid> Owners { get; set; } = new List<Guid>();

        /// <summary>
        /// Tổ chức, cá nhân khai thác đập
        /// </summary>
        public string ExploitName { get; set; }
        public string ExploitAddress { get; set; }
        public string ExploitPhone { get; set; }
        public string ExploitFax { get; set; }
        public string ExploitEmail { get; set; }

        /// <summary>
        /// Html editor
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public string Imange { get; set; }
    }
}
