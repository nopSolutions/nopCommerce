using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;

namespace Nop.Data
{
    public interface IDataProvider
    {
        IDbConnectionFactory GetConnectionFactory();

        void InitConnectionFactory();

        void SetDatabaseInitializer();
    }
}
