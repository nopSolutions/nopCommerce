using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in order according to a product sorting rule
        /// </summary>
        /// <param name="productsQuery">A sequence of products to order</param>
        /// <param name="orderBy">Product sorting rule</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
        public static IOrderedQueryable<Product> OrderBy(this IQueryable<Product> productsQuery, ProductSortingEnum orderBy) 
        {
            return orderBy switch
            {
                ProductSortingEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
                ProductSortingEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
                ProductSortingEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
                ProductSortingEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
                ProductSortingEnum.CreatedOn => productsQuery.OrderByDescending(p => p.CreatedOnUtc),
                _ => productsQuery.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id)
            };
        }
    }
}