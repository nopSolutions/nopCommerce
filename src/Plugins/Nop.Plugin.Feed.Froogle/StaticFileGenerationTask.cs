using System;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.Feed.Froogle
{
    public class StaticFileGenerationTask : ITask
    {
        private readonly IPluginFinder _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
        private readonly ILogger _logger = EngineContext.Current.Resolve<ILogger>();
        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Froogle");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            //plugin
            var plugin = pluginDescriptor.Instance() as FroogleService;
            if (plugin == null)
                return;

            try
            {
                plugin.GenerateStaticFile();
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
            }
        }
    }
}