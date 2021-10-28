using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ExternalAuthenticationController : BaseAdminController
    {
        #region Fields

        protected ExternalAuthenticationSettings ExternalAuthenticationSettings { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IExternalAuthenticationMethodModelFactory ExternalAuthenticationMethodModelFactory { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }

        #endregion

        #region Ctor

        public ExternalAuthenticationController(ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IEventPublisher eventPublisher,
            IExternalAuthenticationMethodModelFactory externalAuthenticationMethodModelFactory,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            ExternalAuthenticationSettings = externalAuthenticationSettings;
            AuthenticationPluginManager = authenticationPluginManager;
            EventPublisher = eventPublisher;
            ExternalAuthenticationMethodModelFactory = externalAuthenticationMethodModelFactory;
            PermissionService = permissionService;
            SettingService = settingService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Methods()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            //prepare model
            var model = ExternalAuthenticationMethodModelFactory
                .PrepareExternalAuthenticationMethodSearchModel(new ExternalAuthenticationMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Methods(ExternalAuthenticationMethodSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ExternalAuthenticationMethodModelFactory.PrepareExternalAuthenticationMethodListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MethodUpdate(ExternalAuthenticationMethodModel model)
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
    }
}