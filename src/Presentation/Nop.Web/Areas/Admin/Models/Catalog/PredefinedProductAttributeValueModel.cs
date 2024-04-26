using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a predefined product attribute value model
/// </summary>
public partial record PredefinedProductAttributeValueModel : BaseNopEntityModel, ILocalizedModel<PredefinedProductAttributeValueLocalizedModel>
{
    #region Ctor

    public PredefinedProductAttributeValueModel()
    {
        Locales = new List<PredefinedProductAttributeValueLocalizedModel>();
    }

    #endregion

    #region Properties

    public int ProductAttributeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustment")]
    public decimal PriceAdjustment { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustment")]
    //used only on the values list page
    public string PriceAdjustmentStr { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.PriceAdjustmentUsePercentage")]
    public bool PriceAdjustmentUsePercentage { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.WeightAdjustment")]
    public decimal WeightAdjustment { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.WeightAdjustment")]
    //used only on the values list page
    public string WeightAdjustmentStr { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Cost")]
    public decimal Cost { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<PredefinedProductAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record PredefinedProductAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name")]
    public string Name { get; set; }
}