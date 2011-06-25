using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Hosting;

namespace Nop.Data
{
    public class SqlCeDataProvider : BaseEfDataProvider
    {
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", HostingEnvironment.MapPath("~/App_Data/"), "");
        }
        public override void SetDatabaseInitializer()
        {
            var initializer = new CreateDatabaseIfNotExists<NopObjectContext>();
            Database.SetInitializer<NopObjectContext>(initializer);
        }
    }
}
