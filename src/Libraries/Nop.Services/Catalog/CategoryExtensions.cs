

using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CategoryExtensions
    {
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <returns>Sorted categories</returns>
        public static IList<Category> SortCategoriesForTree(this IList<Category> source, int parentId)
        {
            var result = new List<Category>();

            var temp = source.ToList().FindAll(c => c.ParentCategoryId == parentId);
            foreach (var cat in temp)
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.Id));
            }
            return result;
        }

    }
}
