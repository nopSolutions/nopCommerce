using System.Data.Entity;
using Nop.Core.Tasks;

namespace Nop.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var dataProviderManager = new DataProviderManager();
            var settings = dataProviderManager.LoadSettings();
            if (settings != null && settings.IsValid())
            {
                var provider = dataProviderManager.LoadDataProvider(settings.DataProvider);
                provider.SetDatabaseInitializer();
            }
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
