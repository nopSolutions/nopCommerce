using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency model
/// </summary>
public partial record CurrencyModel : BaseNopEntityModel, ILocalizedModel<CurrencyLocalizedModel>, IStoreMappingSupportedModel
{
    #region Ctor

    public CurrencyModel()
    {
        Locales = new List<CurrencyLocalizedModel>();

        SelectedStoreIds = new List<int>();
        AvailableStores = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode")]
    public string CurrencyCode { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale")]
    public string DisplayLocale { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate")]
    public decimal Rate { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting")]
    public string CustomFormatting { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
    public bool IsPrimaryExchangeRateCurrency { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryStoreCurrency")]
    public bool IsPrimaryStoreCurrency { get; set; }

    public IList<CurrencyLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.RoundingType")]
    public int RoundingTypeId { get; set; }

    #endregion
}

public partial record CurrencyLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
    public string Name { get; set; }
}