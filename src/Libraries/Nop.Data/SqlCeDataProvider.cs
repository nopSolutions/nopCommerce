using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Data.Initializers;

namespace Nop.Data
{
    public class SqlCeDataProvider : BaseEfDataProvider
    {
        public override IDbConnectionFactory GetConnectionFactory()
        {
            //return new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", HostingEnvironment.MapPath("~/App_Data/"), "");
            return new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
        }
        public override void SetDatabaseInitializer()
        {
            //var initializer = new CreateDatabaseIfNotExists<NopObjectContext>();
            var initializer = new CreateCeDatabaseIfNotExists<NopObjectContext>();
            Database.SetInitializer<NopObjectContext>(initializer);
        }

        public override bool StoredProceduredSupported
        {
            get { return false; }
        }
    }
}
