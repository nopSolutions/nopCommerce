using System.Data.Entity.Infrastructure;
using Nop.Core.Data;

namespace Nop.Data
{
    public interface IEfDataProvider: IDataProvider
    {
        IDbConnectionFactory GetConnectionFactory();

        void InitConnectionFactory();

        void SetDatabaseInitializer();
    }
}
