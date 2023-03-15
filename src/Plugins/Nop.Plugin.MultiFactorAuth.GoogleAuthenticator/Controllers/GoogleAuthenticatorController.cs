using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class GoogleAuthenticatorController : BasePluginController
    {
        #region Fields

        protected readonly GoogleAuthenticatorService _googleAuthenticatorService;
        protected readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;        
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor 

        public GoogleAuthenticatorController(GoogleAuthenticatorService googleAuthenticatorService,
            GoogleAuthenticatorSettings googleAuthenticatorSettings,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext
            )
        {
            _googleAuthenticatorService = googleAuthenticatorService;
            _googleAuthenticatorSettings = googleAuthenticatorSettings;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                QRPixelsPerModule = _googleAuthenticatorSettings.QRPixelsPerModule,
                BusinessPrefix = _googleAuthenticatorSettings.BusinessPrefix
            };
            model.GoogleAuthenticatorSearchModel.HideSearchBlock = await _genericAttributeService
                .GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), GoogleAuthenticatorDefaults.HideSearchBlockAttribute);

            return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //set new settings values
            _googleAuthenticatorSettings.QRPixelsPerModule = model.QRPixelsPerModule;
            _googleAuthenticatorSettings.BusinessPrefix = model.BusinessPrefix;
            await _settingService.SaveSettingAsync(_googleAuthenticatorSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> GoogleAuthenticatorList(GoogleAuthenticatorSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            //get GoogleAuthenticator configuration records
            var configurations = await _googleAuthenticatorService.GetPagedConfigurationsAsync(searchModel.SearchEmail,
                searchModel.Page - 1, searchModel.PageSize);
            var model = new GoogleAuthenticatorListModel().PrepareToGrid(searchModel, configurations, () =>
            {
                //fill in model values from the configuration
                return configurations.Select(configuration => new GoogleAuthenticatorModel
                {
                    Id = configuration.Id,
                    Customer = configuration.Customer,
                    SecretKey = configuration.SecretKey
                });
            });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> GoogleAuthenticatorDelete(GoogleAuthenticatorModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //delete configuration
            var configuration = await _googleAuthenticatorService.GetConfigurationByIdAsync(model.Id);
            if (configuration != null)
            {
                await _googleAuthenticatorService.DeleteConfigurationAsync(configuration);
            }
            var customer = await _customerService.GetCustomerByEmailAsync(configuration.Customer) ??
                await _customerService.GetCustomerByUsernameAsync(configuration.Customer);

            if (customer != null)
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

            return new NullJsonResult();
        }

        #endregion
    }
}
