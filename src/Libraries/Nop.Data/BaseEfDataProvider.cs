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
            //pass some table names to ensure that we have nopCommerce 2.X installed
            var initializer = new CreateTablesIfNotExist<NopObjectContext>(new[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" });
            Database.SetInitializer<NopObjectContext>(initializer);
        }

        public virtual void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }
    }
}
