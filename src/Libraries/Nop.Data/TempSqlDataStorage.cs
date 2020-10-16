using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace Nop.Data.DataProviders
{
    /// <summary>
    /// Represents temporary storage
    /// </summary>
    /// <typeparam name="T">Storage record mapping class</typeparam>
    public class TempSqlDataStorage<T> : TempTable<T>, ITempDataStorage<T> where T : class
    {
        #region Ctor

        public TempSqlDataStorage(string storageName, IQueryable<T> query, Func<DataConnection> dataConnectionFactory)
            : base(dataConnectionFactory(), storageName, query)
        {
            
        }

        #endregion
    }
}