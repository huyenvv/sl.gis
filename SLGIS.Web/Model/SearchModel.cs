using SLGIS.Core.Extension;
using System;

namespace SLGIS.Web
{
    public class SearchModel
    {
        public string FilterText { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now.ToVNDate();
        public DateTime? EndDate { get; set; } = DateTime.Now.ToVNDate();
    }
}
