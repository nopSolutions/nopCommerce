using System.Net;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Services.Stores;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        private readonly IStoreService _storeService;

        public KeepAliveTask(IStoreService storeService)
        {
            this._storeService = storeService;
        }
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var stores = _storeService.GetAllStores();
            foreach (var store in stores)
            {
                string url = store.Url + "keepalive";
                using (var wc = new WebClient())
                {
                    wc.DownloadString(url);
                }
            }
        }
    }
}
