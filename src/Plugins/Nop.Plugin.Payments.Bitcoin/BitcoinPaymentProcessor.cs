using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.Bitcoin;

/// <summary>
/// Bitcoin payment processor
/// </summary>
public class BitcoinPaymentProcessor : BasePlugin//, IBitcoinPaymentMethod
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly BitcoinPaymentSettings _bitcoinPaymentSettings;

    #endregion

    #region Ctor

    public BitcoinPaymentProcessor(ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        ISettingService settingService,
        IWebHelper webHelper,
        BitcoinPaymentSettings bitcoinPaymentSettings)
    {
        _localizationService = localizationService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _bitcoinPaymentSettings = bitcoinPaymentSettings;
    }

    #endregion

    #region Methods



    #endregion

    #region Properties



    #endregion
}