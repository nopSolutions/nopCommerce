using Nop.Core.Caching;
using Nop.Services.Tasks;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Clear cache scheduled task implementation
    /// </summary>
    public partial class ClearCacheTask : IScheduleTask
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public ClearCacheTask(IStaticCacheManager staticCacheManager)
        {
            this._staticCacheManager = staticCacheManager;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _staticCacheManager.Clear();
        }
    }
}
