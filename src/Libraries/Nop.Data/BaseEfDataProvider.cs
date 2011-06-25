using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Nop.Data
{
    public abstract class BaseEfDataProvider:IEfDataProvider
    {
        public abstract IDbConnectionFactory GetConnectionFactory();

        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = GetConnectionFactory();
        }

        public virtual void SetDatabaseInitializer()
        {
            var initializer = new CreateTablesIfNotExists<NopObjectContext>();
            Database.SetInitializer<NopObjectContext>(initializer);
        }

        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }
    }
}
