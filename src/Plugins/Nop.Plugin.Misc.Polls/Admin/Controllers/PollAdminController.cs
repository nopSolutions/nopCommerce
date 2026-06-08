using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Polls.Admin.Factories;
using Nop.Plugin.Misc.Polls.Admin.Models;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Plugin.Misc.Polls.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Polls.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[SaveSelectedTab]
public class PollAdminController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly PollModelFactory _pollModelFactory;
    private readonly PollService _pollService;

    #endregion

    #region Ctor

    public PollAdminController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        PollModelFactory pollModelFactory,
        PollService pollService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
        _pollModelFactory = pollModelFactory;
        _storeMappingService = storeMappingService;
        _pollService = pollService;
    }

    #endregion

    #region Configure

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var pollSettings = await _settingService.LoadSettingAsync<PollSettings>(storeId);

        var model = new ConfigurationModel
        {
            Enabled = pollSettings.Enabled,
            ActiveStoreScopeConfiguration = storeId
        };

        if (storeId > 0)
            model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(pollSettings, settings => settings.Enabled, storeId);

        return View("~/Plugins/Misc.Polls/Admin/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var pollSettings = await _settingService.LoadSettingAsync<PollSettings>(storeId);

        pollSettings.Enabled = model.Enabled;

        await _settingService.SaveSettingOverridablePerStoreAsync(pollSettings, settings => settings.Enabled, model.Enabled_OverrideForStore, storeId, false);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion

    #region Polls

    public IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(PollsDefaults.Permissions.POLLS_VIEW)]
    public async Task<IActionResult> List()
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollSearchModelAsync(new());

        return View("~/Plugins/Misc.Polls/Admin/Views/List.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_VIEW)]
    public async Task<IActionResult> List(PollSearchModel searchModel)
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(new(), null);

        return View("~/Plugins/Misc.Polls/Admin/Views/Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> Create(PollModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var poll = model.ToEntity<Poll>();
            await _pollService.InsertPollAsync(poll);

            //save store mappings
            await _storeMappingService.SaveStoreMappingsAsync(poll, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Polls.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Polls/Admin/Views/Create.cshtml", model);
    }

    [CheckPermission(PollsDefaults.Permissions.POLLS_VIEW)]
    public async Task<IActionResult> Edit(int id)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
        if (poll == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _pollModelFactory.PreparePollModelAsync(null, poll);

        return View("~/Plugins/Misc.Polls/Admin/Views/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> Edit(PollModel model, bool continueEditing)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(model.Id);
        if (poll == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            poll = model.ToEntity(poll);
            await _pollService.UpdatePollAsync(poll);

            //save store mappings
            await _storeMappingService.SaveStoreMappingsAsync(poll, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Polls.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = poll.Id });
        }

        //prepare model
        model = await _pollModelFactory.PreparePollModelAsync(model, poll, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Polls/Admin/Views/Edit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> Delete(int id)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(id);
        if (poll == null)
            return RedirectToAction("List");

        await _pollService.DeletePollAsync(poll);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Polls.Deleted"));

        return RedirectToAction("List");
    }

    #endregion

    #region Poll answer

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_VIEW)]
    public async Task<IActionResult> PollAnswers(PollAnswerSearchModel searchModel)
    {
        //try to get a poll with the specified id
        var poll = await _pollService.GetPollByIdAsync(searchModel.PollId)
            ?? throw new ArgumentException("No poll found with the specified id");

        //prepare model
        var model = await _pollModelFactory.PreparePollAnswerListModelAsync(searchModel, poll);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> PollAnswerUpdate([Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(model.Id)
            ?? throw new ArgumentException("No poll answer found with the specified id");

        pollAnswer = model.ToEntity(pollAnswer);

        await _pollService.UpdatePollAnswerAsync(pollAnswer);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> PollAnswerAdd(int pollId, [Validate] PollAnswerModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //fill entity from model
        await _pollService.InsertPollAnswerAsync(model.ToEntity<PollAnswer>());

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(PollsDefaults.Permissions.POLLS_MANAGE)]
    public async Task<IActionResult> PollAnswerDelete(int id)
    {
        //try to get a poll answer with the specified id
        var pollAnswer = await _pollService.GetPollAnswerByIdAsync(id)
            ?? throw new ArgumentException("No poll answer found with the specified id", nameof(id));

        await _pollService.DeletePollAnswerAsync(pollAnswer);

        return new NullJsonResult();
    }

    #endregion
}