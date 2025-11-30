using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.DataStructures
{
    public class ApiList<T> : List<T>
    {
        public ApiList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        public ApiList(IQueryable<T> source)
        {
            AddRange(source.ToList());
        }

        public int PageIndex { get; }
        public int PageSize { get; }
    }
}
