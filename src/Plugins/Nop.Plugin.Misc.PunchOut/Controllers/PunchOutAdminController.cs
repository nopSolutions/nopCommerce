using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.PunchOut.Models;
using Nop.Plugin.Misc.PunchOut.Models.Identity;
using Nop.Plugin.Misc.PunchOut.Models.Log;
using Nop.Plugin.Misc.PunchOut.Models.Session;
using Nop.Plugin.Misc.PunchOut.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class PunchOutAdminController : BasePluginController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;
    protected readonly PunchOutIdentityService _punchOutIdentityService;
    protected readonly PunchOutLogService _punchOutLogService;
    protected readonly PunchOutService _punchOutService;
    protected readonly PunchOutSettings _punchOutSettings;

    #endregion

    #region Ctor

    public PunchOutAdminController(ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreService storeService,
        IWorkContext workContext,
        PunchOutIdentityService punchOutIdentityService,
        PunchOutLogService punchOutLogService,
        PunchOutService punchOutService,
        PunchOutSettings punchOutSettings)
    {
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeService = storeService;
        _workContext = workContext;
        _punchOutIdentityService = punchOutIdentityService;
        _punchOutLogService = punchOutLogService;
        _punchOutService = punchOutService;
        _punchOutSettings = punchOutSettings;
    }

    #endregion

    #region Methods

    #region Configuration

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        //prepare model
        var model = new ConfigurationModel
        {
            IsActive = _punchOutSettings.IsActive,
            TimeToExpire = _punchOutSettings.TimeToExpire,
            SelectedCustomerRoleIds = _punchOutSettings.RestrictedCustomerRoleIds
        };

        //prepare unavailable customer roles
        var unavailableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
        model.UnavailableCustomerRoles = unavailableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();

        model.PunchOutLogSearchModel.HideSearchBlock = await _genericAttributeService
            .GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideSearchLogBlock);
        model.PunchOutIdentitySearchModel.HideSearchBlock = await _genericAttributeService
            .GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideSearchIdentityBlock);

        model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideGeneralBlock);
        model.HideIdentityBlock = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideIdentityBlock);
        model.HideSessionBlock = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideSessionBlock);
        model.HideLogBlock = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer, PunchOutDefaults.HideLogBlock);

        return View("~/Plugins/Misc.PunchOut/Views/Configuration/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //set new settings values
        _punchOutSettings.IsActive = model.IsActive;
        _punchOutSettings.TimeToExpire = model.TimeToExpire;
        _punchOutSettings.RestrictedCustomerRoleIds = model.SelectedCustomerRoleIds.ToList();

        await _settingService.SaveSettingAsync(_punchOutSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion

    #region Log

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> LogList(PunchOutLogSearchModel searchModel)
    {
        //prepare filter parameters
        var createdFromValue = searchModel.CreatedFrom.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
            : null;
        var createdToValue = searchModel.CreatedTo.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
            : null;

        //get punch out log
        var punchOutLog = await _punchOutLogService.GetPunchOutLogAsync(createdFromUtc: createdFromValue, createdToUtc: createdToValue,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new PunchOutLogListModel().PrepareToGridAsync(searchModel, punchOutLog, () =>
        {
            return punchOutLog.SelectAwait(async logItem => new PunchOutLogModel
            {
                Id = logItem.Id,
                SessionId = logItem.SessionId,
                Direction = logItem.Direction.ToString(),
                MessageType = logItem.MessageType.ToString(),
                CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc)
            });
        });

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> LogView(int id)
    {
        //try to get log item with the passed identifier
        var logItem = await _punchOutLogService.GetPunchOutLogByIdAsync(id);
        if (logItem == null)
            return RedirectToAction("Configure", "PunchOutAdmin");

        var model = new PunchOutLogModel
        {
            Id = logItem.Id,
            Url = logItem.Url,
            SessionId = logItem.SessionId,
            Direction = logItem.Direction.ToString(),
            Error = logItem.Error,
            RawXml = _htmlFormatter.FormatText(logItem.RawXml),
            MessageType = logItem.MessageType.ToString(),
            Identity = logItem.Identity,
            CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc)
        };

        return View("~/Plugins/Misc.PunchOut/Views/Log/View.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Delete(int id)
    {
        //try to get log item with the passed identifier
        var logItem = await _punchOutLogService.GetPunchOutLogByIdAsync(id);
        if (logItem != null)
        {
            await _punchOutLogService.DeleteLogItemAsync(logItem);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.PunchOut.Log.Deleted"));
        }

        return RedirectToAction("Configure", "PunchOutAdmin");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> ClearAll()
    {
        await _punchOutLogService.ClearLogAsync();

        return Json(new { Result = true });
    }

    #endregion

    #region Identity

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> IdentityList(PunchOutIdentitySearchModel searchModel)
    {
        //get punch out identities
        var punchOutIdentities = await _punchOutIdentityService.GetPunchOutIdentitiesAsync(identity: searchModel.Identity,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = new PunchOutIdentityListModel().PrepareToGrid(searchModel, punchOutIdentities, () =>
        {
            return punchOutIdentities.Select(identityItem => new PunchOutIdentityModel
            {
                Id = identityItem.Id,
                Identity = identityItem.Identity,
                SharedSecret = identityItem.SharedSecretHash
            });
        });

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> IdentityAdd(PunchOutIdentityModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        await _punchOutIdentityService.AddPunchOutIdentityAsync(model.Identity, model.SharedSecret);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> IdentityUpdate(PunchOutIdentityModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a identity with the specified id
        var identity = await _punchOutIdentityService.GetPunchOutIdentityByIdAsync(model.Id)
            ?? throw new ArgumentException("No punchout identity found with the specified id");

        identity.Identity = model.Identity;
        identity.SharedSecretHash = model.SharedSecret;

        await _punchOutIdentityService.UpdatePunchOutIdentityAsync(identity);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> IdentityDelete(int id)
    {
        //try to get a punchout identity with the specified id
        var identity = await _punchOutIdentityService.GetPunchOutIdentityByIdAsync(id)
            ?? throw new ArgumentException("No punchout identity found with the specified id", nameof(id));

        await _punchOutIdentityService.DeletePunchOutIdentityAsync(identity.Id);

        return new NullJsonResult();
    }

    #endregion

    #region Session

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> SessionList(PunchOutSessionSearchModel searchModel)
    {
        //get punch out sessions
        var punchOutSessions = await _punchOutService.GetAllPunchOutSessionAsync(
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        if (punchOutSessions is null)
            return ErrorJson("Error retrieving saved sessions");

        //prepare store names (to avoid loading for each item)
        var storeNames = (await _storeService.GetAllStoresAsync()).ToDictionary(store => store.Id, store => store.Name);

        //prepare grid model
        var model = await new PunchOutSessionListModel().PrepareToGridAsync(searchModel, punchOutSessions, () =>
        {
            return punchOutSessions.SelectAwait(async sessionItem =>
            {
                var model = new PunchOutSessionModel
                {
                    SessionId = sessionItem.SessionId,
                    IsActive = sessionItem.IsActive,
                    BuyerCookie = sessionItem.BuyerCookie,
                    CustomerId = sessionItem.CustomerId,
                    CustomerEmail = (await _customerService.GetCustomerByIdAsync(sessionItem.CustomerId))?.Email ?? string.Empty,
                    StoreId = sessionItem.StoreId,
                    StoreName = storeNames.TryGetValue(sessionItem.StoreId, out var value) ? value : "Deleted",
                    CreatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(sessionItem.CreatedOnUtc, DateTimeKind.Utc)
                };
                return model;
            });
        });

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> CloseSession(int customerId, int storeId)
    {
        await _genericAttributeService
            .SaveAttributeAsync<string>(new Customer { Id = customerId }, PunchOutDefaults.PunchOutSessionAttribute, null, storeId);

        return new NullJsonResult();
    }

    #endregion

    #endregion
}
