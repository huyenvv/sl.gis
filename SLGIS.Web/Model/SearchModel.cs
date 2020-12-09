﻿using System;

namespace SLGIS.Web.Model
{
    public class SearchModel
    {
        public string FilterText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
