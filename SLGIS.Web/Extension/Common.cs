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

        public static string ToVNDateTimeString(this DateTime date)
        {
            if (date == null) return "";

            return date.ToString("dd/MM/yyyy HH:mm:ss");
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
                    {".png", "image/png"},
                    {".jpg", "image/jpeg"},
                    {".jpeg", "image/jpeg"},
                    {".gif", "image/gif"},                    
                    {".mp4", "video/mp4"},
                    {".avi", "video/x-msvideo"},
                    {".wmv", "video/x-ms-wmv"},
                    {".3gp", "video/3gpp"},
                    {".mp3", "audio/mpeg"},
                    {".wav", "audio/wav"},
                    {".7z", "application/x-7z-compressed"},
                    {".zip", "application/zip"},
                    {".txt", "text/plain"},
                    {".doc", "application/msword"},
                    {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                    {".ppt", "application/vnd.ms-powerpoint"},
                    {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                    {".pdf", "application/pdf"},
                };
            }
        }
    }
}
