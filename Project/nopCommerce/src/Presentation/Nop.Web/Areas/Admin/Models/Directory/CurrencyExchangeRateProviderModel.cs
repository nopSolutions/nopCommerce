using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a currency exchange rate provider model
/// </summary>
public partial record CurrencyExchangeRateProviderModel : BaseNopModel
{
    #region Ctor

    public CurrencyExchangeRateProviderModel()
    {
        ExchangeRates = new List<CurrencyExchangeRateModel>();
        ExchangeRateProviders = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyRateAutoUpdateEnabled")]
    public bool AutoUpdateEnabled { get; set; }

    public IList<CurrencyExchangeRateModel> ExchangeRates { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Currencies.Fields.ExchangeRateProvider")]
    public string ExchangeRateProvider { get; set; }
    public IList<SelectListItem> ExchangeRateProviders { get; set; }

    #endregion
}