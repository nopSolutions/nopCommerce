using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.DataStructures
{
    public class ApiList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public ApiList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }
    }
}