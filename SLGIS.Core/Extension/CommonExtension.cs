using System;

namespace SLGIS.Core.Extension
{
    public static class CommonExtension
    {
        public static DateTime ToVNDate(this DateTime date)
        {
            return date.ToUniversalTime().AddHours(7);
        }

        public static DateTime? ToVNDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToUniversalTime().AddHours(7);
            }

            return null;
        }
    }
}
