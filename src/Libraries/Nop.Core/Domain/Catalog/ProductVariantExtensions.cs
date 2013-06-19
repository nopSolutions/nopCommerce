using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Catalog
{
    public static class ProductVariantExtensions
    {
        public static int[] ParseRequiredProductIds(this Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (String.IsNullOrEmpty(product.RequiredProductIds))
                return new int[0];

            var ids = new List<int>();

            foreach (var idStr in product.RequiredProductIds
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
            {
                int id = 0;
                if (int.TryParse(idStr, out id))
                    ids.Add(id);
            }

            return ids.ToArray();
        }
    }
}
