using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record SystemInfoModel : BaseNopModel
{
    public SystemInfoModel()
    {
        Headers = new List<HeaderModel>();
        LoadedAssemblies = new List<LoadedAssembly>();
    }

    [NopResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
    public string AspNetInfo { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
    public bool IsFullTrust { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.NopVersion")]
    public string NopVersion { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
    public string OperatingSystem { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
    public DateTime ServerLocalTime { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
    public string ServerTimeZone { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
    public DateTime UtcTime { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.CurrentUserTime")]
    public DateTime CurrentUserTime { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.CurrentStaticCacheManager")]
    public string CurrentStaticCacheManager { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
    public string HttpHost { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.Headers")]
    public IList<HeaderModel> Headers { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
    public IList<LoadedAssembly> LoadedAssemblies { get; set; }

    [NopResourceDisplayName("Admin.System.SystemInfo.AzureBlobStorageEnabled")]
    public bool AzureBlobStorageEnabled { get; set; }

    public partial record HeaderModel : BaseNopModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public partial record LoadedAssembly : BaseNopModel
    {
        public string FullName { get; set; }
        public string Location { get; set; }
        public bool IsDebug { get; set; }
        public DateTime? BuildDate { get; set; }
    }
}