using Nop.Core.Plugins;
using Nop.Services.Tasks;

namespace Nop.Plugin.Feed.Froogle
{
    public class StaticFileGenerationTask : ITask
    {
        private readonly IPluginFinder _pluginFinder;

        public StaticFileGenerationTask(IPluginFinder pluginFinder)
        {
            this._pluginFinder = pluginFinder;
        }

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Froogle");
            if (pluginDescriptor == null)
                return;

            //plugin
            var plugin = pluginDescriptor.Instance() as FroogleService;
            if (plugin == null)
                return;

            plugin.GenerateStaticFile();
        }
    }
}