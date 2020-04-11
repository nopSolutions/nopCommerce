using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceApiResponse<T>
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public List<T> Results { get; set; } = new List<T>();
    }
}
