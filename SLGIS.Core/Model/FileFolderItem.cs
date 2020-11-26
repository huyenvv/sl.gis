using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SLGIS.Core
{
    /// <summary>
    ///  File or Folder
    /// </summary>
    public class FileFolderItem : BaseEntity
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Trường yêu cầu bắt buộc")]
        public string Title { get; set; }

        [JsonIgnore]
        public string FullPath { get; set; }

        public bool IsFolder { get; set; }

        public bool IsPrivate { get; set; } = true;

        public Guid? ParentId { get; set; }

        public double Size { get; set; }

        [JsonIgnore]
        public string UpdatedBy { get; set; }

        public DateTime Updated { get; set; }

        public string GetExtension()
        {
            if (string.IsNullOrEmpty(this.Title) || this.IsFolder)
            {
                return string.Empty;
            }

            return Path.GetExtension(this.Title);
        }
    }
}
