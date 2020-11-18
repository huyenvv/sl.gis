using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SLGIS.Web
{
    public class UploadModel
    {
        public Guid? ParentId { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
