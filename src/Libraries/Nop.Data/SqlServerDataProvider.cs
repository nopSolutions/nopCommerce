using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Data.Initializers;

namespace Nop.Data
{
    public class SqlServerDataProvider : BaseEfDataProvider
    {
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        public override void SetDatabaseInitializer()
        {
            //pass some table names to ensure that we have nopCommerce 2.X installed
            var initializer = new CreateTablesIfNotExist<NopObjectContext>(new[] { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" });
            Database.SetInitializer<NopObjectContext>(initializer);
        }
    }
}
