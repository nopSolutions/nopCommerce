using System.Net;
using Nop.Core;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : IScheduleTask
    {
        private readonly IStoreContext _storeContext;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="storeContext">Store context</param>
        public KeepAliveTask(IStoreContext storeContext)
        {
            this._storeContext = storeContext;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var url = _storeContext.CurrentStore.Url + "keepalive/index";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
