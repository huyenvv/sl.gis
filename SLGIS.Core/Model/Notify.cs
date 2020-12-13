using MongoDB.Bson;
using SLGIS.Core.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SLGIS.Core
{
    /// <summary>
    /// Thông báo của sở cho các thủy điện
    /// </summary>
    public class Notify : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Files { get; set; } = new List<string>();
        public List<KeyValuePair<Guid, List<ReadNotifyInfo>>> ReadPlants { get; set; } = new List<KeyValuePair<Guid, List<ReadNotifyInfo>>>();

        /// <summary>
        /// Is sent to all users
        /// </summary>
        public bool IsAll { get; set; } = true;
        public List<Guid> ToPlantIds { get; set; } = new List<Guid>();

        public bool CanEdit()
        {
            var secondsOfDay = 24 * 60 * 60;
            return (DateTime.Now.ToVNDate() - Created.ToUniversalTime()).TotalSeconds < secondsOfDay;
        }

        public bool IsRead(Guid plantId, ObjectId userId)
        {
            if (userId == null) return false;

            return ReadPlants.Any(m => m.Key == plantId && m.Value.Any(n => n.UserId == userId.ToString()));
        }

        public DateTime? GetReadTime(Guid plantId, ObjectId userId)
        {
            if (userId == null) return null;

            var readTime = ReadPlants.FirstOrDefault(m => m.Key == plantId && m.Value.Any(n => n.UserId == userId.ToString())).Value;
            if (readTime != null)
            {
                return readTime.FirstOrDefault(m => m.UserId == userId.ToString()).ReadDate.ToUniversalTime();
            }

            return null;
        }
    }

    public class ReadNotifyInfo
    {
        public string UserId { get; set; }
        public DateTime ReadDate { get; private set; } = DateTime.Now.ToVNDate();
    }
}
