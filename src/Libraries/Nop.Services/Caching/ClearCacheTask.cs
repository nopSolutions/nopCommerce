using System;
using System.Diagnostics;
using System.Xml;
using Nop.Core.Caching;
using Nop.Core.Tasks;

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
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();
        }
    }
}
