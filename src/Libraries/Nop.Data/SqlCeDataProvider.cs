using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Hosting;
using Nop.Core;

namespace Nop.Data
{
    public class SqlCeDataProvider : IDataProvider
    {
        public IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", HostingEnvironment.MapPath("~/App_Data/"), "");
        }

        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = GetConnectionFactory();
        }

        public void SetDatabaseInitializer()
        {
            var initializer = new CreateDatabaseIfNotExists<NopObjectContext>();
            Database.SetInitializer<NopObjectContext>(initializer);
        }
    }
}
