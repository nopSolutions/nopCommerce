using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Plugin.Misc.Forums.Admin.Factories;
using Nop.Plugin.Misc.Forums.Admin.Models;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.Forums.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[SaveSelectedTab]

public class ForumController : BasePluginController
{
    #region Fields

    private readonly ForumModelFactory _forumModelFactory;
    private readonly ForumService _forumService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ForumController(ForumModelFactory forumModelFactory,
        ForumService forumService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    #region Configure

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        //prepare model
        var model = await _forumModelFactory.PrepareConfigurationModelAsync();

        return View("~/Plugins/Misc.Forums/Admin/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (ModelState.IsValid)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeScope);
            forumSettings = model.ToSettings(forumSettings);

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumsEnabled, model.ForumsEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, model.RelativeDateTimeFormattingEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ShowCustomersPostCount, model.ShowCustomersPostCount_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowGuestsToCreatePosts, model.AllowGuestsToCreatePosts_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowGuestsToCreateTopics, model.AllowGuestsToCreateTopics_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowCustomersToEditPosts, model.AllowCustomersToEditPosts_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowCustomersToDeletePosts, model.AllowCustomersToDeletePosts_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowPostVoting, model.AllowPostVoting_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.MaxVotesPerDay, model.MaxVotesPerDay_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowCustomersToManageSubscriptions, model.AllowCustomersToManageSubscriptions_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.TopicsPageSize, model.TopicsPageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumEditor, model.ForumEditor_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.SignaturesEnabled, model.SignaturesEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, model.ActiveDiscussionsFeedEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, model.ActiveDiscussionsFeedCount_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumFeedsEnabled, model.ForumFeedsEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumFeedCount, model.ForumFeedCount_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.SearchResultsPageSize, model.SearchResultsPageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsPageSize, model.ActiveDiscussionsPageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ShowCaptcha, model.ShowCaptcha_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction(nameof(Configure));
        }

        //prepare model
        model = await _forumModelFactory.PrepareConfigurationModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Forums/Admin/Views/Configure.cshtml", model);
    }


    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> ShowCaptcha(bool showCaptcha)
    {
        if (!showCaptcha)
            return Json(new { Result = string.Empty });

        var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>();
        if (captchaSettings.Enabled)
            return Json(new { Result = string.Empty });

        var url = Url.Action("GeneralCommon", "Setting");
        var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Configuration.ShowCaptcha.Warning"), url);
        return Json(new { Result = warning });
    }

    #endregion

    #region List

    public IActionResult Index()
    {
        return RedirectToAction(nameof(List));
    }

    [CheckPermission(ForumDefaults.Permissions.FORUMS_VIEW)]
    public async Task<IActionResult> List()
    {
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupSearchModelAsync(new());

        return View("~/Plugins/Misc.Forums/Admin/Views/List.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_VIEW)]
    public async Task<IActionResult> ForumGroupList(ForumGroupSearchModel searchModel)
    {
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_VIEW)]
    public async Task<IActionResult> ForumList(ForumSearchModel searchModel)
    {
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(searchModel.ForumGroupId)
            ?? throw new ArgumentException("No forum group found with the specified id");

        //prepare model
        var model = await _forumModelFactory.PrepareForumListModelAsync(searchModel, forumGroup);

        return Json(model);
    }

    #endregion

    #region Create

    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> CreateForumGroup()
    {
        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupModelAsync(new(), null);

        return View("~/Plugins/Misc.Forums/Admin/Views/CreateForumGroup.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> CreateForumGroup(ForumGroupModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var forumGroup = model.ToEntity<ForumGroup>();
            forumGroup.CreatedOnUtc = DateTime.UtcNow;
            forumGroup.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.InsertForumGroupAsync(forumGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumGroup.Added"));

            return continueEditing ? RedirectToAction(nameof(CreateForumGroup), new { forumGroup.Id }) : RedirectToAction(nameof(List));
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumGroupModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Forums/Admin/Views/CreateForumGroup.cshtml", model);
    }

    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> CreateForum()
    {
        //prepare model
        var model = await _forumModelFactory.PrepareForumModelAsync(new(), null);

        return View("~/Plugins/Misc.Forums/Admin/Views/CreateForum.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> CreateForum(ForumModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var forum = model.ToEntity<Forum>();
            forum.CreatedOnUtc = DateTime.UtcNow;
            forum.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.InsertForumAsync(forum);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Forum.Added"));

            return continueEditing ? RedirectToAction(nameof(EditForum), new { forum.Id }) : RedirectToAction(nameof(List));
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Forums/Admin/Views/CreateForum.cshtml", model);
    }

    #endregion

    #region Edit

    [CheckPermission(ForumDefaults.Permissions.FORUMS_VIEW)]
    public async Task<IActionResult> EditForumGroup(int id)
    {
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
        if (forumGroup == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _forumModelFactory.PrepareForumGroupModelAsync(null, forumGroup);

        return View("~/Plugins/Misc.Forums/Admin/Views/EditForumGroup.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> EditForumGroup(ForumGroupModel model, bool continueEditing)
    {
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(model.Id);
        if (forumGroup == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            forumGroup = model.ToEntity(forumGroup);
            forumGroup.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.UpdateForumGroupAsync(forumGroup);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumGroup.Updated"));

            return continueEditing ? RedirectToAction(nameof(EditForumGroup), forumGroup.Id) : RedirectToAction(nameof(List));
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumGroupModelAsync(model, forumGroup, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Forums/Admin/Views/EditForumGroup.cshtml", model);
    }

    [CheckPermission(ForumDefaults.Permissions.FORUMS_VIEW)]
    public async Task<IActionResult> EditForum(int id)
    {
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(id);
        if (forum == null)
            return RedirectToAction(nameof(List));

        //prepare model
        var model = await _forumModelFactory.PrepareForumModelAsync(null, forum);

        return View("~/Plugins/Misc.Forums/Admin/Views/EditForum.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> EditForum(ForumModel model, bool continueEditing)
    {
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(model.Id);
        if (forum == null)
            return RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            forum = model.ToEntity(forum);
            forum.UpdatedOnUtc = DateTime.UtcNow;
            await _forumService.UpdateForumAsync(forum);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Forum.Updated"));

            return continueEditing ? RedirectToAction(nameof(EditForum), forum.Id) : RedirectToAction(nameof(List));
        }

        //prepare model
        model = await _forumModelFactory.PrepareForumModelAsync(model, forum, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.Forums/Admin/Views/EditForum.cshtml", model);
    }

    #endregion

    #region Delete

    [HttpPost]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> DeleteForumGroup(int id)
    {
        //try to get a forum group with the specified id
        var forumGroup = await _forumService.GetForumGroupByIdAsync(id);
        if (forumGroup == null)
            return RedirectToAction(nameof(List));

        await _forumService.DeleteForumGroupAsync(forumGroup);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumGroup.Deleted"));

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    [CheckPermission(ForumDefaults.Permissions.FORUMS_MANAGE)]
    public async Task<IActionResult> DeleteForum(int id)
    {
        //try to get a forum with the specified id
        var forum = await _forumService.GetForumByIdAsync(id);
        if (forum == null)
            return RedirectToAction(nameof(List));

        await _forumService.DeleteForumAsync(forum);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Forum.Deleted"));

        return RedirectToAction(nameof(List));
    }

    #endregion

    #endregion
}