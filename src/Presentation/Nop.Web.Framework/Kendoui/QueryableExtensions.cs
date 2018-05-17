using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Nop.Web.Framework.Kendoui
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filter a collection
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="queryable">Collection</param>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Result</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> queryable, Filter filter)
        {
            if (filter != null && filter.Logic != null)
            {
                // Collect a flat list of all filters
                var filters = filter.All();

                // Get all filter values as array (needed by the Where method of Dynamic Linq)
                var values = filters.Select(f => f.Value).ToArray();

                // Create a predicate expression e.g. Field1 = @0 And Field2 > @1
                var predicate = filter.ToExpression(filters);

                // Use the Where method of Dynamic Linq to filter the data
                queryable = queryable.Where(predicate, values);
            }

            return queryable;
        }

        /// <summary>
        /// Sort a collection
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="queryable">Collection</param>
        /// <param name="sort">Sort parameters</param>
        /// <returns>Result</returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, IEnumerable<Sort> sort)
        {
            if (sort != null && sort.Any())
            {
                // Create ordering expression e.g. Field1 asc, Field2 desc
                var ordering = string.Join(",", sort.Select(s => s.ToExpression()));

                // Use the OrderBy method of Dynamic Linq to sort the data
                return queryable.OrderBy(ordering);
            }

            return queryable;
        }
    }
}
