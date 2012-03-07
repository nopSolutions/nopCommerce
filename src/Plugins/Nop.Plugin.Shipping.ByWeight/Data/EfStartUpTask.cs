using System.Data.Entity;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Shipping.ByWeight.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<ShippingByWeightObjectContext>(null);
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
}
