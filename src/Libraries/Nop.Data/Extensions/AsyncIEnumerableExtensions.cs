namespace System.Linq
{
    public static class AsyncIEnumerableExtensions
    {
        /// <summary>
        /// Projects each element of an async-enumerable sequence into a new form by applying
        /// an asynchronous selector function to each member of the source sequence and awaiting
        /// the result.
        /// </summary>
        /// <typeparam name="TSource"> The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TResult">
        /// The type of the elements in the result sequence, obtained by running the selector
        /// function for each element in the source sequence and awaiting the result.
        /// </typeparam>
        /// <param name="source">A sequence of elements to invoke a transform function on</param>
        /// <param name="predicate">An asynchronous transform function to apply to each source element</param>
        /// <returns>
        /// An async-enumerable sequence whose elements are the result of invoking the transform
        /// function on each element of the source sequence and awaiting the result
        /// </returns>
        public static IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<TResult>> predicate)
        {
            return source.ToAsyncEnumerable().SelectAwait(predicate);
        }

        /// <summary>
        /// Returns the first element of an async-enumerable sequence that satisfies the
        /// condition in the predicate, or a default value if no element satisfies the condition
        /// in the predicate
        /// </summary>
        /// <typeparam name="TSource">The type of element in the sequence</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="predicate">An asynchronous predicate to invoke and await on each element of the sequence</param>
        /// <returns>
        /// A Task containing the first element in the sequence that satisfies the predicate,
        /// or a default value if no element satisfies the predicate
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<TSource> FirstOrDefaultAwaitAsync<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<bool>> predicate)
        {
            return source.ToAsyncEnumerable().FirstOrDefaultAwaitAsync(predicate).AsTask();
        }

        /// <summary>
        /// Determines whether all elements in an async-enumerable sequence satisfy a condition
        /// </summary>
        /// <typeparam name="TSource">The type of element in the sequence</typeparam>
        /// <param name="source">An sequence whose elements to apply the predicate to</param>
        /// <param name="predicate">An asynchronous predicate to apply to each element of the source sequence</param>
        /// <returns>
        /// A Task containing a value indicating whether all elements in the sequence
        /// pass the test in the specified predicate
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<bool> AllAwaitAsync<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<bool>> predicate)
        {
            return source.ToAsyncEnumerable().AllAwaitAsync(predicate).AsTask();
        }

        /// <summary>
        /// Projects each element of an async-enumerable sequence into an async-enumerable
        /// sequence and merges the resulting async-enumerable sequences into one async-enumerable
        /// sequence
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source sequence</typeparam>
        /// <typeparam name="TResult">The type of elements in the projected inner sequences and the merged result sequence</typeparam>
        /// <param name="source">An async-enumerable sequence of elements to project</param>
        /// <param name="predicate">An asynchronous selector function to apply to each element of the source sequence</param>
        /// <returns>
        /// An async-enumerable sequence whose elements are the result of invoking the one-to-many
        /// transform function on each element of the source sequence and awaiting the result
        /// </returns>
        public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, Task<IList<TResult>>> predicate)
        {
            async ValueTask<IAsyncEnumerable<TResult>> getAsyncEnumerable(TSource items)
            {
                var rez = await predicate(items);
                return rez.ToAsyncEnumerable();
            }

            return source.ToAsyncEnumerable().SelectManyAwait(getAsyncEnumerable);
        }

        /// <summary>
        /// Projects each element of an async-enumerable sequence into an async-enumerable
        /// sequence and merges the resulting async-enumerable sequences into one async-enumerable
        /// sequence
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source sequence</typeparam>
        /// <typeparam name="TResult">The type of elements in the projected inner sequences and the merged result sequence</typeparam>
        /// <param name="source">An async-enumerable sequence of elements to project</param>
        /// <param name="predicate">An asynchronous selector function to apply to each element of the source sequence</param>
        /// <returns>
        /// An async-enumerable sequence whose elements are the result of invoking the one-to-many
        /// transform function on each element of the source sequence and awaiting the result
        /// </returns>
        public static IAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, Task<IEnumerable<TResult>>> predicate)
        {
            async ValueTask<IAsyncEnumerable<TResult>> getAsyncEnumerable(TSource items)
            {
                var rez = await predicate(items);
                return rez.ToAsyncEnumerable();
            }

            return source.ToAsyncEnumerable().SelectManyAwait(getAsyncEnumerable);
        }

        /// <summary>
        /// Filters the elements of an async-enumerable sequence based on an asynchronous
        /// predicate
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">An async-enumerable sequence whose elements to filter</param>
        /// <param name="predicate">An asynchronous predicate to test each source element for a condition</param>
        /// <returns>
        /// An async-enumerable sequence that contains elements from the input sequence that
        /// satisfy the condition
        /// </returns>
        public static IAsyncEnumerable<TSource> WhereAwait<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<bool>> predicate)
        {
            return source.ToAsyncEnumerable().WhereAwait(predicate);
        }

        /// <summary>
        /// Determines whether any element in an async-enumerable sequence satisfies a condition
        /// </summary>
        /// <typeparam name="TSource">The type of element in the sequence</typeparam>
        /// <param name="source">An async-enumerable sequence whose elements to apply the predicate to</param>
        /// <param name="predicate">An asynchronous predicate to apply to each element of the source sequence</param>
        /// <returns>
        /// A Task containing a value indicating whether any elements in the source
        /// sequence pass the test in the specified predicate
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<bool> AnyAwaitAsync<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<bool>> predicate)
        {
            return source.ToAsyncEnumerable().AnyAwaitAsync(predicate).AsTask();
        }

        /// <summary>
        /// Returns the only element of an async-enumerable sequence that satisfies the condition
        /// in the asynchronous predicate, or a default value if no such element exists,
        /// and reports an exception if there is more than one element in the async-enumerable
        /// sequence that matches the predicate
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source sequence</typeparam>
        /// <param name="source">Source async-enumerable sequence</param>
        /// <param name="predicate">An asynchronous predicate that will be applied to each element of the source sequence</param>
        /// <returns>
        /// Task containing the only element in the async-enumerable sequence that satisfies
        /// the condition in the asynchronous predicate, or a default value if no such element
        /// exists
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<TSource> SingleOrDefaultAwaitAsync<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<bool>> predicate)
        {
            return source.ToAsyncEnumerable().SingleOrDefaultAwaitAsync(predicate).AsTask();
        }

        /// <summary>
        /// Creates a list from an async-enumerable sequence
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <param name="source">The source async-enumerable sequence to get a list of elements for</param>
        /// <returns>
        /// An async-enumerable sequence containing a single element with a list containing
        /// all the elements of the source sequence
        /// </returns>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static Task<List<TSource>> ToListAsync<TSource>(this IEnumerable<TSource> source)
        {
            return source.ToAsyncEnumerable().ToListAsync().AsTask();
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key obtained
        /// by invoking a transform function on each element and awaiting the result
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector</typeparam>
        /// <param name="source">An async-enumerable sequence of values to order</param>
        /// <param name="keySelector">An asynchronous function to extract a key from an element</param>
        /// <returns>
        /// An ordered async-enumerable sequence whose elements are sorted in descending
        /// order according to a key
        /// </returns>
        public static IOrderedAsyncEnumerable<TSource> OrderByDescendingAwait<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector)
        {
            return source.ToAsyncEnumerable().OrderByDescendingAwait(keySelector);
        }

        /// <summary>
        /// Groups the elements of an async-enumerable sequence and selects the resulting
        /// elements by using a specified function
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the grouping key computed for each element in the source sequence</typeparam>
        /// <typeparam name="TElement">The type of the elements within the groups computed for each element in the source sequence</typeparam>
        /// <param name="source">An async-enumerable sequence whose elements to group</param>
        /// <param name="keySelector">An asynchronous function to extract the key for each element</param>
        /// <param name="elementSelector">An asynchronous function to map each source element to an element in an async-enumerable group</param>
        /// <returns>
        /// A sequence of async-enumerable groups, each of which corresponds to a unique
        /// key value, containing all elements that share that same key value
        /// </returns>
        public static IAsyncEnumerable<IAsyncGrouping<TKey, TElement>> GroupByAwait<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector,
            Func<TSource, ValueTask<TElement>> elementSelector)
        {
            return source.ToAsyncEnumerable().GroupByAwait(keySelector, elementSelector);
        }

        /// <summary>
        /// Applies an accumulator function over an async-enumerable sequence, returning
        /// the result of the aggregation as a single element in the result sequence. The
        /// specified seed value is used as the initial accumulator value
        /// </summary>
        /// <typeparam name="TSource">specified seed value is used as the initial accumulator value</typeparam>
        /// <typeparam name="TAccumulate">The type of the result of aggregation</typeparam>
        /// <param name="source">An async-enumerable sequence to aggregate over</param>
        /// <param name="seed">The initial accumulator value</param>
        /// <param name="accumulator">An asynchronous accumulator function to be invoked and awaited on each element</param>
        /// <returns>A Task containing the final accumulator value</returns>
        public static ValueTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(
            this IEnumerable<TSource> source, TAccumulate seed,
            Func<TAccumulate, TSource, ValueTask<TAccumulate>> accumulator)
        {
            return source.ToAsyncEnumerable().AggregateAwaitAsync(seed, accumulator);
        }

        /// <summary>
        /// Creates a dictionary from an async-enumerable sequence using the specified asynchronous
        /// key and element selector functions
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the dictionary key computed for each element in the source sequence</typeparam>
        /// <typeparam name="TElement">The type of the dictionary value computed for each element in the source sequence</typeparam>
        /// <param name="source">An async-enumerable sequence to create a dictionary for</param>
        /// <param name="keySelector">An asynchronous function to extract a key from each element</param>
        /// <param name="elementSelector">An asynchronous transform function to produce a result element value from each element</param>
        /// <returns>
        /// A Task containing a dictionary mapping unique key values onto the corresponding
        /// source sequence's element
        /// </returns>
        public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector,
            Func<TSource, ValueTask<TElement>> elementSelector) where TKey : notnull
        {
            return source.ToAsyncEnumerable().ToDictionaryAwaitAsync(keySelector, elementSelector);
        }

        /// <summary>
        /// Groups the elements of an async-enumerable sequence according to a specified
        /// key selector function
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the grouping key computed for each element in the source sequence</typeparam>
        /// <param name="source">An async-enumerable sequence whose elements to group</param>
        /// <param name="keySelector">An asynchronous function to extract the key for each element</param>
        /// <returns>
        /// A sequence of async-enumerable groups, each of which corresponds to a unique
        /// key value, containing all elements that share that same key value
        /// </returns>
        public static IAsyncEnumerable<IAsyncGrouping<TKey, TSource>> GroupByAwait<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> keySelector)
        {
            return source.ToAsyncEnumerable().GroupByAwait(keySelector);
        }

        /// <summary>
        /// Computes the sum of a sequence of System.Decimal values that are obtained by
        /// invoking a transform function on each element of the source sequence and awaiting
        /// the result
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source sequence</typeparam>
        /// <param name="source">A sequence of values that are used to calculate a sum</param>
        /// <param name="selector">An asynchronous transform function to apply to each element</param>
        /// <returns>A Task containing the sum of the values in the source sequence</returns>
        public static ValueTask<decimal> SumAwaitAsync<TSource>(this IEnumerable<TSource> source,
            Func<TSource, ValueTask<decimal>> selector)
        {
            return source.ToAsyncEnumerable().SumAwaitAsync(selector);
        }
    }
}