using Nop.Core.Caching;
using Nop.Services.Tasks;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Clear cache schedueled task implementation
    /// </summary>
    public partial class ClearCacheTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();
        }
    }
}
