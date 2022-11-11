using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;

namespace System.Linq
{
    public static class AsyncIQueryableExtensions
    {
        /// <summary>
        /// Determines whether all the elements of a sequence satisfy a condition
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence whose elements to test for a condition</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// true if every element of the source sequence passes the test in the specified
        /// predicate, or if the sequence is empty; otherwise, false
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<bool> AllAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return AsyncExtensions.AllAsync(source, predicate);
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence whose elements to test for a condition</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// true if any elements in the source sequence pass the test in the specified predicate;
        /// otherwise, false
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.AnyAsync(source) : AsyncExtensions.AnyAsync(source, predicate);
        }

        #region Average

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, int>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, int?>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, long>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, long?>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, float>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, float?>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, double>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, double?>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, decimal>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        /// <summary>
        /// Computes the average of a sequence that is obtained by
        /// invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to calculate the average of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the average of the sequence of values
        /// </returns>
        public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, decimal?>> predicate)
        {
            return AsyncExtensions.AverageAsync(source, predicate);
        }

        #endregion

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default
        /// equality comparer
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An sequence in which to locate item</param>
        /// <param name="item">The object to locate in the sequence</param>
        /// <returns>
        /// true if the input sequence contains an element that has the specified value;
        /// otherwise, false
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<bool> ContainsAsync<TSource>(this IQueryable<TSource> source, TSource item)
        {
            return AsyncExtensions.ContainsAsync(source, item);
        }

        /// <summary>
        /// Returns the number of elements in the specified sequence that satisfies a condition
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An sequence that contains the elements to be counted</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// The number of elements in the sequence that satisfies the condition in the predicate
        /// function
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.CountAsync(source) : AsyncExtensions.CountAsync(source, predicate);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">An sequence to return an element from</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the first element in source that passes the test in predicate
        /// </returns>
        public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.FirstAsync(source) : AsyncExtensions.FirstAsync(source, predicate);
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">Source</param>
        /// <param name="predicate">Predicate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the default(TSource) if source is empty; otherwise, the first element in source
        /// </returns>
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.FirstOrDefaultAsync(source) : AsyncExtensions.FirstOrDefaultAsync(source, predicate);
        }

        /// <summary>
        /// Returns an System.Int64 that represents the number of elements in a sequence
        /// that satisfy a condition
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An sequence that contains the elements to be counted</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// The number of elements in source that satisfy the condition in the predicate
        /// function
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.LongCountAsync(source) : AsyncExtensions.LongCountAsync(source, predicate);
        }

        /// <summary>
        /// Returns the maximum value in a generic sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to determine the maximum of</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the maximum value in the sequence
        /// </returns>
        public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source)
        {
            return AsyncExtensions.MaxAsync(source);
        }

        /// <summary>
        /// Invokes a projection function on each element of a generic sequence
        /// and returns the maximum resulting value
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by selector</typeparam>
        /// <param name="source">A sequence of values to determine the maximum of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the maximum value in the sequence
        /// </returns>
        public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> predicate)
        {
            return AsyncExtensions.MaxAsync(source, predicate);
        }

        /// <summary>
        /// Returns the minimum value in a generic sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values to determine the minimum of</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the minimum value in the sequence
        /// </returns>
        public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source)
        {
            return AsyncExtensions.MinAsync(source);
        }

        /// <summary>
        /// Invokes a projection function on each element of a generic sequence
        /// and returns the minimum resulting value
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the function represented by selector</typeparam>
        /// <param name="source">A sequence of values to determine the minimum of</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the minimum value in the sequence
        /// </returns>
        public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> predicate)
        {
            return AsyncExtensions.MinAsync(source, predicate);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition,
        /// and throws an exception if more than one such element exists
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">An sequence to return a single element from</param>
        /// <param name="predicate">A function to test an element for a condition</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the single element of the input sequence that satisfies the condition in predicate
        /// </returns>
        public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.SingleAsync(source) : AsyncExtensions.SingleAsync(source, predicate);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or
        /// a default value if no such element exists; this method throws an exception if
        /// more than one element satisfies the condition
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">A sequence to return a single element from</param>
        /// <param name="predicate">A function to test an element for a condition</param>
        /// <returns>
        /// The single element of the input sequence that satisfies the condition in predicate,
        /// or default(TSource) if no such element is found
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate = null)
        {
            return predicate == null ? AsyncExtensions.SingleOrDefaultAsync(source) : AsyncExtensions.SingleOrDefaultAsync(source, predicate);
        }

        #region Sum

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, decimal>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, decimal?>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, double?>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, float?>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, double>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, int>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, int?>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, long>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, long?>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        /// <summary>
        /// Computes the sum of the sequence that is obtained
        /// by invoking a projection function on each element of the input sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">A sequence of values of type TSource</param>
        /// <param name="predicate">A projection function to apply to each element</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sum of the projected values
        /// </returns>
        public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, float>> predicate)
        {
            return AsyncExtensions.SumAsync(source, predicate);
        }

        #endregion

        /// <summary>
        /// Asynchronously loads data from query to a dictionary
        /// </summary>
        /// <typeparam name="TSource">Query element type</typeparam>
        /// <typeparam name="TKey">Dictionary key type</typeparam>
        /// <typeparam name="TElement">Dictionary element type</typeparam>
        /// <param name="source">Source query</param>
        /// <param name="keySelector">Source element key selector</param>
        /// <param name="elementSelector">Dictionary element selector</param>
        /// <param name="comparer">Dictionary key comparer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary with query results
        /// </returns>
        public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null) where TKey : notnull
        {
            return comparer == null
                ? AsyncExtensions.ToDictionaryAsync(source, keySelector, elementSelector)
                : AsyncExtensions.ToDictionaryAsync(source, keySelector, elementSelector, comparer);
        }

        /// <summary>
        /// Asynchronously loads data from query to a dictionary
        /// </summary>
        /// <typeparam name="TSource">Query element type</typeparam>
        /// <typeparam name="TKey">Dictionary key type</typeparam>
        /// <param name="source">Source query</param>
        /// <param name="keySelector">Source element key selector</param>
        /// <param name="comparer">Dictionary key comparer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary with query results
        /// </returns>
        public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IQueryable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null) where TKey : notnull
        {
            return comparer == null
                ? AsyncExtensions.ToDictionaryAsync(source, keySelector)
                : AsyncExtensions.ToDictionaryAsync(source, keySelector, comparer);
        }

        /// <summary>
        /// Asynchronously loads data from query to a list
        /// </summary>
        /// <typeparam name="TSource">Query element type</typeparam>
        /// <param name="source">Source query</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list with query results
        /// </returns>
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
        {
            return AsyncExtensions.ToListAsync(source);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            //min allowed page size is 1
            pageSize = Math.Max(pageSize, 1);

            var count = await source.CountAsync();

            var data = new List<T>();

            if (!getOnlyTotalCount)
                data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());

            return new PagedList<T>(data, pageIndex, pageSize, count);
        }
    }
}
