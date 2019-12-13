using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<T> AsList<T>(this IEnumerable<T> source) =>
            source == null ? new List<T>() : source is List<T> ? (List<T>)source : source.ToList();
    }
}
