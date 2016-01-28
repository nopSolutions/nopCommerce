

using Nop.Core.Caching;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
namespace Nop.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static void RemoveByPattern(this ICacheManager obj, string pattern, IEnumerable<string> keys)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in keys.Where(p => regex.IsMatch(p)))
                obj.Remove(key);
        }
    }
}
