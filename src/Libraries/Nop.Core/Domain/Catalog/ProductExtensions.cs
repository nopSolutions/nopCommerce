using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;

namespace Nop.Core.Domain.Catalog
{
    public static class ProductExtensions
    {
        public static ProductVariant MinimalPriceProductVariant(this Product product)
        {
            var productVariants = product.ProductVariants.ToList();
            productVariants.Sort(new GenericComparer<ProductVariant>
                ("Price", GenericComparer<ProductVariant>.SortOrder.Ascending));
            return productVariants.Count > 0 ? productVariants[0] : null;
        }
    }
}
