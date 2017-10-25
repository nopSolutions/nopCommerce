namespace Nop.Core.Plugins
{
    /// <summary>
    /// Base plugin
    /// 所有插件的基类，它实现了接口IPlugin，功能主要是插件的安装和卸载。
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        public virtual void Install() 
        {
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public virtual void Uninstall() 
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
        }

    }
}
