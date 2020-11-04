using System;
using System.Collections.Generic;
using System.IO;

namespace SLGIS.Web
{
    public static class Common
    {
        public const int MaxUploadSize = 524288000;
        public static string ToLocalDateString(this DateTime date, string lang = "")
        {
            if (date == null) return "";

            if (lang == "en")
                return date.ToLocalTime().ToString("MM/dd/yyyy");

            return date.ToLocalTime().ToString("dd/MM/yyyy");
        }

        public static string GetCode(this DateTime date)
        {
            if (date == null) return "";

            return date.ToLocalTime().ToString("yyyyMMdd");
        }

        public static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return MimeTypes[ext];
        }

        private static Dictionary<string, string> MimeTypes
        {
            get
            {
                return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".zip", "application/zip"},
                {".mp4", "video/mp4"},
                {".avi", "video/x-msvideo"},
                {".wmv", "video/x-ms-wmv"},
                {".3gp", "video/3gpp"},
                {".mp3", "audio/mpeg"},
                {".aac", "audio/aac"},
                {".wav", "audio/wav"},
                {".weba", "audio/webm"},
                {".oga", "audio/ogg"},
                {".opus", "audio/opus"},
            };
            }
        }
    }
}
