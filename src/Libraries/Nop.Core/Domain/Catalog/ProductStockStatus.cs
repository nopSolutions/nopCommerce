using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public enum ProductStockStatus
    {
        OutOfStock = 0,
        LowStock = 1,
        InStock = 2,
    }
}