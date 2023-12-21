using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a measure weight model
/// </summary>
public partial record MeasureWeightModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword")]
    public string SystemKeyword { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.Ratio")]
    public decimal Ratio { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Measures.Weights.Fields.IsPrimaryWeight")]
    public bool IsPrimaryWeight { get; set; }

    #endregion
}