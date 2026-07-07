using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Data;

namespace Nop.Services.Catalog;

public static class ProductExtensions
{
    #region Utilities

    private static IQueryable<Product> OrderByLocalizedName(this IQueryable<Product> productsQuery,
        IRepository<LocalizedProperty> localizedPropertyRepository,
        int languageId)
    {
        var query =
            from product in productsQuery
            join localizedProperty in localizedPropertyRepository.Table on new
            {
                Id = product.Id,
                LangId = languageId,
                KeyGroup = nameof(Product),
                Key = nameof(Product.Name)
            } equals new
            {
                Id = localizedProperty.EntityId,
                LangId = localizedProperty.LanguageId,
                KeyGroup = localizedProperty.LocaleKeyGroup,
                Key = localizedProperty.LocaleKey
            } into localizedProperties
            from localizedProperty in localizedProperties.DefaultIfEmpty(new LocalizedProperty { LocaleValue = product.Name })
            select new
            {
                SortName = localizedProperty == null ? product.Name : localizedProperty.LocaleValue,
                Product = product
            };

        return
            from item in query
            orderby item.SortName
            select item.Product;
    }

    private static IQueryable<Product> OrderByLocalizedNameDescending(this IQueryable<Product> productsQuery,
        IRepository<LocalizedProperty> localizedPropertyRepository,
        int languageId)
    {
        var query =
            from product in productsQuery
            join localizedProperty in localizedPropertyRepository.Table on new
            {
                Id = product.Id,
                LangId = languageId,
                KeyGroup = nameof(Product),
                Key = nameof(Product.Name)
            } equals new
            {
                Id = localizedProperty.EntityId,
                LangId = localizedProperty.LanguageId,
                KeyGroup = localizedProperty.LocaleKeyGroup,
                Key = localizedProperty.LocaleKey
            } into localizedProperties
            from localizedProperty in localizedProperties.DefaultIfEmpty(new LocalizedProperty { LocaleValue = product.Name })
            select new
            {
                SortName = localizedProperty == null ? product.Name : localizedProperty.LocaleValue,
                Product = product
            };

        return
            from item in query
            orderby item.SortName descending
            select item.Product;
    }

    private static IQueryable<Product> OrderByIndexedResults(this IQueryable<Product> productsQuery, IList<int> indexedResults)
    {
        return
            from product in productsQuery
            join indexed in indexedResults.Select((id, index) => new { Index = index, Id = id }).AsQueryable() on product.Id equals indexed.Id into sortedQuery
            from sorted in sortedQuery.DefaultIfEmpty()
            orderby sorted == null ? int.MaxValue : sorted.Index
            select product;
    }

    #endregion

    /// <summary>
    /// Sorts the elements of a sequence in order according to a product sorting rule
    /// </summary>
    /// <param name="productsQuery">A sequence of products to order</param>
    /// <param name="orderBy">Product sorting rule</param>
    /// <param name="localizedPropertyRepository">Localized property repository; pass null to sort by general (non-localized) product names</param>
    /// <param name="languageId">Language identifier; pass 0 to sort by general (non-localized) product names</param>
    /// <param name="indexedResults">List of product identifiers whose index matches the product's position in the sorted list</param>
    /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
    /// <remarks>
    /// If <paramref name="orderBy"/> is set to <c>Position</c> and passed <paramref name="productsQuery"/> is ordered sorting rule will be skipped
    /// </remarks>
    public static IQueryable<Product> OrderBy(this IQueryable<Product> productsQuery,
        ProductSortingEnum orderBy,
        IRepository<LocalizedProperty> localizedPropertyRepository = null,
        int languageId = 0,
        IList<int> indexedResults = null)
    {
        return orderBy switch
        {
            ProductSortingEnum.NameAsc when localizedPropertyRepository is not null => productsQuery.OrderByLocalizedName(localizedPropertyRepository, languageId),
            ProductSortingEnum.NameDesc when localizedPropertyRepository is not null => productsQuery.OrderByLocalizedNameDescending(localizedPropertyRepository, languageId),
            ProductSortingEnum.Position when indexedResults?.Any() == true => productsQuery.OrderByIndexedResults(indexedResults),
            ProductSortingEnum.NameAsc => productsQuery.OrderBy(p => p.Name),
            ProductSortingEnum.NameDesc => productsQuery.OrderByDescending(p => p.Name),
            ProductSortingEnum.PriceAsc => productsQuery.OrderBy(p => p.Price),
            ProductSortingEnum.PriceDesc => productsQuery.OrderByDescending(p => p.Price),
            ProductSortingEnum.CreatedOn => productsQuery.OrderByDescending(p => p.CreatedOnUtc),
            ProductSortingEnum.Position when productsQuery is IOrderedQueryable => productsQuery,
            _ => productsQuery.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id)
        };
    }
}