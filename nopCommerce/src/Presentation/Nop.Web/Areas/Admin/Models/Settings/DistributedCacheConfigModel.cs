using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents distributed cache configuration model
/// </summary>
public partial record DistributedCacheConfigModel : BaseNopModel, IConfigModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.DistributedCacheType")]
    public SelectList DistributedCacheTypeValues { get; set; }
    public int DistributedCacheType { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.ConnectionString")]
    public string ConnectionString { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.SchemaName")]
    public string SchemaName { get; set; } = "dbo";

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.TableName")]
    public string TableName { get; set; } = "DistributedCache";

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.InstanceName")]
    public string InstanceName { get; protected set; } = string.Empty;

    [NopResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.PublishIntervalMs")]
    public int PublishIntervalMs { get; protected set; }

    #endregion
}