using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    /// <summary>
    /// Represents the startup task that sets a database initializer
    /// </summary>
    public partial class DataProviderStartupTask : IStartupTask
    {
        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //set database initializer
            if (DataSettingsManager.LoadSettings()?.IsValid ?? false)
            {
                var provider = EngineContext.Current.Resolve<IDataProvider>()
                    ?? throw new NopException("No data provider found");
                
#if EF6
                provider.SetDatabaseInitializer();
#endif
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets order of this startup task implementation
        /// </summary>
        public int Order => -1000;  //ensure that this task is run first

        #endregion
    }
}