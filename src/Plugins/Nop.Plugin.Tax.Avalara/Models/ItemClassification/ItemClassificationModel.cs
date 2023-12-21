using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a item classification model
/// </summary>
public record ItemClassificationModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Product")]
    public int ProductId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Product")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.HSClassificationRequestId")]
    public string HSClassificationRequestId { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Country")]
    public int CountryId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Country")]
    public string CountryName { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.HSCode")]
    public string HSCode { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.UpdatedDate")]
    public DateTime UpdatedDate { get; set; }

    #endregion
}