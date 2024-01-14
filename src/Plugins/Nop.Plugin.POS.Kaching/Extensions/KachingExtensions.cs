using System;
using System.Text.RegularExpressions;

namespace Nop.Plugin.POS.Kaching.Extensions
{
    public static class KachingExtensions
    {
        public static string StripHTML(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string stripped = Regex.Replace(input, "<.*?>", String.Empty);
            stripped = stripped.Replace("[b]", "").Replace("[/b]", "").Replace("[br]", ",").Replace("[br/]", ",");

            return stripped.Trim();
        }

        public static bool Between(this DateTime dt, DateTime start, DateTime end)
        {
            if (start < end) return dt >= start && dt <= end;
            return dt >= end && dt <= start;
        }
    }
}
