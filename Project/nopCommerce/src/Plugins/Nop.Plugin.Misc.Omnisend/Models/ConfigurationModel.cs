using System.ComponentModel.DataAnnotations;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Omnisend.Models;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.Omnisend.Fields.ApiKey")]
    [DataType(DataType.Password)]
    public string ApiKey { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Omnisend.Fields.UseTracking")]
    public bool UseTracking { get; set; }

    public bool BlockSyncContacts { get; set; }

    public bool BlockSyncProducts { get; set; }

    public bool BlockSyncOrders { get; set; }

    public IList<BatchResponse> Batches { get; set; } = new List<BatchResponse>();

    #endregion
}