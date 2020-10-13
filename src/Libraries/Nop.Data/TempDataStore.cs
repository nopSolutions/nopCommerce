using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;

namespace Nop.Data.DataProviders
{
    //TODO: IDisposeAsync https://docs.microsoft.com/ru-ru/dotnet/standard/garbage-collection/implementing-disposeasync
    /// <summary>
    /// Represents temporary storage
    /// </summary>
    /// <typeparam name="T">Storage record mapping class</typeparam>
    public class TempSqlDataStorage<T> : ITempDataStorage<T> where T : class
    {
        #region Fields

        private readonly IDisposable _disposableFactory;
        private readonly IDisposable _disposableResource;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates new temporary table and populate it using data from provided query. 
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="query">Name of temporary table</param>
        /// <param name="dataConnectionFactory">Query to get records to populate created table with initial data.</param>
        public TempSqlDataStorage(string storageName, IQueryable<T> query, Func<DataConnection> dataConnectionFactory)
        {
            if (dataConnectionFactory is null)
                throw new ArgumentNullException(nameof(dataConnectionFactory));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var dataConnection = dataConnectionFactory();

            var tmpTable = dataConnection.CreateTempTable<T>(storageName, query);

            ElementType = tmpTable.AsQueryable().ElementType;
            Expression = tmpTable.AsQueryable().Expression;
            Provider = tmpTable.AsQueryable().Provider;

            _disposableResource = tmpTable;
            _disposableFactory = dataConnection;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _disposableResource.Dispose();
            _disposableFactory.Dispose();
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        #endregion
    }
}