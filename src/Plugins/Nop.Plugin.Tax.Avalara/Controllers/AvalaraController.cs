using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Models.Configuration;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class AvalaraController : BasePluginController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AvalaraController(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            CurrencySettings currencySettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _currencySettings = currencySettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(string testTaxResult = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare common properties
            var model = new ConfigurationModel
            {
                AccountId = _avalaraTaxSettings.AccountId,
                LicenseKey = _avalaraTaxSettings.LicenseKey,
                CompanyCode = _avalaraTaxSettings.CompanyCode,
                UseSandbox = _avalaraTaxSettings.UseSandbox,
                CommitTransactions = _avalaraTaxSettings.CommitTransactions,
                ValidateAddress = _avalaraTaxSettings.ValidateAddress,
                TaxOriginAddressTypeId = (int)_avalaraTaxSettings.TaxOriginAddressType,
                EnableLogging = _avalaraTaxSettings.EnableLogging,
                GetTaxRateByAddressOnly = _avalaraTaxSettings.GetTaxRateByAddressOnly,
                TestTaxResult = testTaxResult
            };
            model.IsConfigured = !string.IsNullOrEmpty(_avalaraTaxSettings.AccountId) && !string.IsNullOrEmpty(_avalaraTaxSettings.LicenseKey);
            model.TaxOriginAddressTypes = (await TaxOriginAddressType.DefaultTaxAddress.ToSelectListAsync(false))
                .Select(type => new SelectListItem(type.Text, type.Value)).ToList();
            model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), AvalaraTaxDefaults.HideGeneralBlock);
            model.HideLogBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), AvalaraTaxDefaults.HideLogBlock);

            //prepare address model
            await _baseAdminModelFactory.PrepareCountriesAsync(model.TestAddress.AvailableCountries);
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.TestAddress.AvailableStates, model.TestAddress.CountryId);

            //prepare tax transaction log model
            model.TaxTransactionLogSearchModel.SetGridPageSize();

            //get active account companies
            var activeCompanies = model.IsConfigured ? await _avalaraTaxManager.GetAccountCompaniesAsync() : null;
            if (activeCompanies?.Any() ?? false)
            {
                model.Companies = activeCompanies.OrderBy(company => company.isDefault ?? false ? 0 : 1).Select(company => new SelectListItem
                {
                    Text = company.isTest ?? false ? $"{company.name} (Test)" : company.name,
                    Value = company.companyCode
                }).ToList();
            }

            var defaultCompanyCode = _avalaraTaxSettings.CompanyCode;
            if (!model.Companies.Any())
            {
                //add the special item for 'there are no companies' with empty guid value
                var noCompaniesText = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.Company.NotExist");
                model.Companies.Add(new SelectListItem { Text = noCompaniesText, Value = Guid.Empty.ToString() });
                defaultCompanyCode = Guid.Empty.ToString();
            }
            else if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                defaultCompanyCode = model.Companies.FirstOrDefault()?.Value;

            //set the default company
            model.CompanyCode = defaultCompanyCode;
            _avalaraTaxSettings.CompanyCode = defaultCompanyCode;
            await _settingService.SaveSettingAsync(_avalaraTaxSettings);

            //display warning in case of company currency differ from the primary store currency
            var primaryCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var selectedCompany = activeCompanies?.FirstOrDefault(company => company.companyCode.Equals(defaultCompanyCode));
            if (!selectedCompany?.baseCurrencyCode?.Equals(primaryCurrency?.CurrencyCode, StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.Company.Currency.Warning"),
                    selectedCompany.name, selectedCompany.baseCurrencyCode, primaryCurrency?.CurrencyCode);
                _notificationService.WarningNotification(warning);
            }

            return View("~/Plugins/Tax.Avalara/Views/Configuration/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _avalaraTaxSettings.AccountId = model.AccountId;
            _avalaraTaxSettings.LicenseKey = model.LicenseKey;
            _avalaraTaxSettings.CompanyCode = model.CompanyCode;
            _avalaraTaxSettings.UseSandbox = model.UseSandbox;
            _avalaraTaxSettings.CommitTransactions = model.CommitTransactions;
            _avalaraTaxSettings.ValidateAddress = model.ValidateAddress;
            _avalaraTaxSettings.TaxOriginAddressType = (TaxOriginAddressType)model.TaxOriginAddressTypeId;
            _avalaraTaxSettings.EnableLogging = model.EnableLogging;
            _avalaraTaxSettings.GetTaxRateByAddressOnly = model.GetTaxRateByAddressOnly;
            await _settingService.SaveSettingAsync(_avalaraTaxSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("verifyCredentials")]
        public async Task<IActionResult> VerifyCredentials()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //verify credentials 
            var result = await _avalaraTaxManager.PingAsync();
            if (result?.authenticated ?? false)
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.VerifyCredentials.Verified"));
            else
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.VerifyCredentials.Declined"));

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("testTax")]
        public async Task<IActionResult> TestTaxRequest(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //get result
            var transaction = await _avalaraTaxManager.CreateTestTaxTransactionAsync(new Address
            {
                City = model.TestAddress?.City,
                CountryId = model.TestAddress?.CountryId,
                Address1 = model.TestAddress?.Address1,
                ZipPostalCode = model.TestAddress?.ZipPostalCode,
                StateProvinceId = model.TestAddress?.StateProvinceId
            });

            var testTaxResult = string.Empty;
            if (transaction?.totalTax != null)
            {
                //display tax rates by jurisdictions
                testTaxResult = $"Total tax rate: {transaction.totalTax:0.00}% {Environment.NewLine}";
                if (transaction.summary?.Any() ?? false)
                {
                    testTaxResult = transaction.summary.Aggregate(testTaxResult, (resultString, rate) =>
                        $"{resultString}Jurisdiction: {rate?.jurisName}, Tax rate: {(rate?.rate ?? 0) * 100:0.00}% {Environment.NewLine}");
                }
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TestTax.Success"));
            }
            else
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TestTax.Error"));

            return await Configure(testTaxResult);
        }

        public async Task<IActionResult> ChangeOriginAddressType(int typeId)
        {
            var message = (TaxOriginAddressType)typeId switch
            {
                TaxOriginAddressType.ShippingOrigin => string.Format(await _localizationService
                    .GetResourceAsync("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.ShippingOrigin.Warning"), Url.Action("Shipping", "Setting")),
                TaxOriginAddressType.DefaultTaxAddress => string.Format(await _localizationService
                    .GetResourceAsync("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.DefaultTaxAddress.Warning"), Url.Action("Tax", "Setting")),
                _ => null
            };

            return Json(new { Result = message });
        }

        #endregion
    }
}