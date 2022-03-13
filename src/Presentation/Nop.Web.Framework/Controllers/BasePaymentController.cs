
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for payment plugins
    /// </summary>
    public abstract class BasePaymentController : BasePluginController
    {
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        protected BasePaymentController() { }

        protected BasePaymentController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion
    }
}
