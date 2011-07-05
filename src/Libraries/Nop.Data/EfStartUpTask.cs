using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;

namespace Nop.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IEfDataProvider>();
                if (provider == null)
                    throw new NopException("No EfDataProvider found");
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
