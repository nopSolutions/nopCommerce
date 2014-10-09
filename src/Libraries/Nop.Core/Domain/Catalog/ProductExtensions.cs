using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Product extensions
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Parse "required product Ids" property
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>A list of required product IDs</returns>
        public static int[] ParseRequiredProductIds(this Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (String.IsNullOrEmpty(product.RequiredProductIds))
                return new int[0];

            var ids = new List<int>();

            foreach (var idStr in product.RequiredProductIds
                .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
            {
                int id;
                if (int.TryParse(idStr, out id))
                    ids.Add(id);
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Product product)
        {
            return IsAvailable(product, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Product product, DateTime dateTime)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
            {
                return false;
            }

            if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
            {
                return false;
            }

            return true;
        }
    }
}
