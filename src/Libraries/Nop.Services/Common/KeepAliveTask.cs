using System.Net;
using Nop.Core.Domain;
using Nop.Core.Infrastructure;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var storeInformationSettings = EngineContext.Current.Resolve<StoreInformationSettings>();
            string url = storeInformationSettings.StoreUrl + "keepalive";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
