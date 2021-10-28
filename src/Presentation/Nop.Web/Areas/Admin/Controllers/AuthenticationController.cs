using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class AuthenticationController : BaseAdminController
    {
        #region Fields

        protected ExternalAuthenticationSettings ExternalAuthenticationSettings { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IExternalAuthenticationMethodModelFactory ExternalAuthenticationMethodModelFactory { get; }
        protected IMultiFactorAuthenticationMethodModelFactory MultiFactorAuthenticationMethodModelFactory { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected MultiFactorAuthenticationSettings MultiFactorAuthenticationSettings { get; }

        #endregion

        #region Ctor

        public AuthenticationController(ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IEventPublisher eventPublisher,
            IExternalAuthenticationMethodModelFactory externalAuthenticationMethodModelFactory,
            IMultiFactorAuthenticationMethodModelFactory multiFactorAuthenticationMethodModelFactory,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            IPermissionService permissionService,
            ISettingService settingService,
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
        {
            ExternalAuthenticationSettings = externalAuthenticationSettings;
            AuthenticationPluginManager = authenticationPluginManager;
            EventPublisher = eventPublisher;
            ExternalAuthenticationMethodModelFactory = externalAuthenticationMethodModelFactory;
            MultiFactorAuthenticationMethodModelFactory = multiFactorAuthenticationMethodModelFactory;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            PermissionService = permissionService;
            SettingService = settingService;
            MultiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        }

        #endregion

        #region External Authentication

        public virtual async Task<IActionResult> ExternalMethods()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            //prepare model
            var model = ExternalAuthenticationMethodModelFactory
                .PrepareExternalAuthenticationMethodSearchModel(new ExternalAuthenticationMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExternalMethods(ExternalAuthenticationMethodSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ExternalAuthenticationMethodModelFactory.PrepareExternalAuthenticationMethodListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExternalMethodUpdate(ExternalAuthenticationMethodModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var method = await AuthenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (AuthenticationPluginManager.IsPluginActive(method))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    ExternalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ExternalAuthenticationSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    ExternalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ExternalAuthenticationSettings);
                }
            }

            var pluginDescriptor = method.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion

        #region Multi-factor Authentication

        public virtual async Task<IActionResult> MultiFactorMethods()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            //prepare model
            var model = MultiFactorAuthenticationMethodModelFactory
                .PrepareMultiFactorAuthenticationMethodSearchModel(new MultiFactorAuthenticationMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MultiFactorMethods(MultiFactorAuthenticationMethodSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await MultiFactorAuthenticationMethodModelFactory.PrepareMultiFactorAuthenticationMethodListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MultiFactorMethodUpdate(MultiFactorAuthenticationMethodModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageMultifactorAuthenticationMethods))
                return AccessDeniedView();

            var method = await MultiFactorAuthenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (MultiFactorAuthenticationPluginManager.IsPluginActive(method))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    MultiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(MultiFactorAuthenticationSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    MultiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(MultiFactorAuthenticationSettings);
                }
            }

            var pluginDescriptor = method.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion
    }
}