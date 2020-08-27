using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents the app settings model
    /// </summary>
    public partial class AppSettingsModel : BaseNopModel
    {
        #region Ctor

        public AppSettingsModel()
        {
            CacheConfigModel = new CacheConfigModel();
            HostingConfigModel = new HostingConfigModel();
            RedisConfigModel = new RedisConfigModel();
            AzureBlobConfigModel = new AzureBlobConfigModel();
            InstallationConfigModel = new InstallationConfigModel();
            PluginConfigModel = new PluginConfigModel();
            CommonConfigModel = new CommonConfigModel();
        }

        #endregion

        #region Properties

        public CacheConfigModel CacheConfigModel { get; set; }

        public HostingConfigModel HostingConfigModel { get; set; }

        public RedisConfigModel RedisConfigModel { get; set; }

        public AzureBlobConfigModel AzureBlobConfigModel { get; set; }

        public InstallationConfigModel InstallationConfigModel { get; set; }

        public PluginConfigModel PluginConfigModel { get; set; }

        public CommonConfigModel CommonConfigModel { get; set; }

        #endregion
    }
}