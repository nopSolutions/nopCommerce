using System.Data.Common;
using LinqToDB;
using LinqToDB.Interceptors;
using StackExchange.Profiling.Data;

namespace Nop.Data.DataProviders.Interceptors
{
    /// <summary>
    /// Represents a class allows MiniProfiler to collect SQL execution timings
    /// </summary>
    public partial class UnwrapProfilerInterceptor : UnwrapDataObjectInterceptor
    {
        // as interceptor is thread-safe, we will create
        // and use single instance of it
        public static readonly IInterceptor Instance = new UnwrapProfilerInterceptor();

        public override DbConnection UnwrapConnection(IDataContext dataContext, DbConnection connection)
        {
            return connection is ProfiledDbConnection c ? c.WrappedConnection : connection;
        }

        public override DbTransaction UnwrapTransaction(IDataContext dataContext, DbTransaction transaction)
        {
            return transaction is ProfiledDbTransaction t ? t.WrappedTransaction : transaction;
        }

        public override DbCommand UnwrapCommand(IDataContext dataContext, DbCommand command)
        {
            return command is ProfiledDbCommand c ? c.InternalCommand : command;
        }

        public override DbDataReader UnwrapDataReader(IDataContext dataContext, DbDataReader dataReader)
        {
            return dataReader is ProfiledDbDataReader dr ? dr.WrappedReader : dataReader;
        }
    }
}