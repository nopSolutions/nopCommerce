using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a plugin configuration model
    /// </summary>
    public partial record PluginConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup")]
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup")]
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UseUnsafeLoadAssembly")]
        public bool UseUnsafeLoadAssembly { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy")]
        public bool UsePluginsShadowCopy { get; set; }

        #endregion
    }
}