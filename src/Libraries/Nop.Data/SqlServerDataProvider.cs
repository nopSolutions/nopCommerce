using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Hosting;
using Nop.Core;

namespace Nop.Data
{
    public class SqlServerDataProvider : IDataProvider
    {
        public IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = GetConnectionFactory();
        }
        
        public void SetDatabaseInitializer()
        {
            var initializer = new CreateTablesIfNotExists<NopObjectContext>();
            Database.SetInitializer<NopObjectContext>(initializer);
        }
    }
}
