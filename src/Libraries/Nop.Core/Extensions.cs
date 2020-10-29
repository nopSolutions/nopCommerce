using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Core
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            var count = await source.ToAsyncEnumerable().CountAsync();

            var data = new List<T>();

            if (!getOnlyTotalCount)
                data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToAsyncEnumerable().ToListAsync());

            return new PagedList<T>(data, pageIndex, pageSize, count);
        }
    }
}
