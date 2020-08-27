using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents Redis configuration model
    /// </summary>
    public partial class RedisConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.ConnectionString")]
        public string ConnectionString { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.DatabaseId")]
        [UIHint("Int32Nullable")]
        public int? DatabaseId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.UseCaching")]
        public bool UseCaching { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.StoreDataProtectionKeys")]
        public bool StoreDataProtectionKeys { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.StorePluginsInfo")]
        public bool StorePluginsInfo { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Redis.IgnoreTimeoutException")]
        public bool IgnoreTimeoutException { get; set; }

        #endregion
    }
}