using System.Threading.Tasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents a schedule task to download tax rates
    /// </summary>
    public class DownloadTaxRatesTask : IScheduleTask
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;

        #endregion

        #region Ctor

        public DownloadTaxRatesTask(AvalaraTaxManager avalaraTaxManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            await _avalaraTaxManager.DownloadTaxRatesAsync();
        }

        #endregion
    }
}