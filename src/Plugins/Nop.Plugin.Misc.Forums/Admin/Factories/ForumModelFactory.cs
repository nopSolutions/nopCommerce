using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Misc.Forums.Admin.Models;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Forums.Admin.Factories;

/// <summary>
/// Represents the forum model factory implementation
/// </summary>
public class ForumModelFactory
{
    #region Fields

    private readonly ForumService _forumService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ForumModelFactory(
        ForumService forumService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _forumService = forumService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare forum search model
    /// </summary>
    /// <param name="searchModel">Forum search model</param>
    /// <returns>Forum search model</returns>
    private ForumSearchModel PrepareForumSearchModel(ForumSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare configuration model
    /// </summary>
    /// <param name="model">Configuration model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the configuration model
    /// </returns>
    public async Task<ConfigurationModel> PrepareConfigurationModelAsync(ConfigurationModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeId);

        //fill in model values from the entity
        model ??= forumSettings.ToSettingsModel<ConfigurationModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
        model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

        if (storeId > 0)
        {
            model.ForumsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
            model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
            model.ShowCustomersPostCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowCustomersPostCount, storeId);
            model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
            model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
            model.AllowCustomersToEditPosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToEditPosts, storeId);
            model.AllowCustomersToDeletePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToDeletePosts, storeId);
            model.AllowPostVoting_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
            model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
            model.AllowCustomersToManageSubscriptions_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeId);
            model.TopicsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
            model.ForumEditor_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
            model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
            model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
            model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
            model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
            model.ForumFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
            model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
            model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);
            model.ShowCaptcha_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowCaptcha, storeId);
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        model.HideCommonBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, ForumDefaults.HideCommonBlockAttributeName);
        model.HidePermissionsBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, ForumDefaults.HidePermissionsBlockAttributeName);
        model.HidePageSizesBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, ForumDefaults.HidePageSizesBlockAttributeName);
        model.HideFeedsBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, ForumDefaults.HideFeedsBlockAttributeName);

        return model;
    }

    /// <summary>
    /// Prepare forum group search model
    /// </summary>
    /// <param name="searchModel">Forum group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group search model
    /// </returns>
    public Task<ForumGroupSearchModel> PrepareForumGroupSearchModelAsync(ForumGroupSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search model
        PrepareForumSearchModel(searchModel.ForumSearch);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged forum group list model
    /// </summary>
    /// <param name="searchModel">Forum group search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group list model
    /// </returns>
    public async Task<ForumGroupListModel> PrepareForumGroupListModelAsync(ForumGroupSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get forum groups
        var forumGroups = (await _forumService.GetAllForumGroupsAsync()).ToPagedList(searchModel);

        //prepare list model
        var model = await new ForumGroupListModel().PrepareToGridAsync(searchModel, forumGroups, () =>
        {
            return forumGroups.SelectAwait(async forumGroup =>
            {
                //fill in model values from the entity
                var forumGroupModel = forumGroup.ToModel<ForumGroupModel>();

                //convert dates to the user time
                forumGroupModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forumGroup.CreatedOnUtc, DateTimeKind.Utc);

                return forumGroupModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare forum group model
    /// </summary>
    /// <param name="model">Forum group model</param>
    /// <param name="forumGroup">Forum group</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum group model
    /// </returns>
    public Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (forumGroup != null)
            model ??= forumGroup.ToModel<ForumGroupModel>();

        //set default values for the new model
        if (forumGroup == null)
            model.DisplayOrder = 1;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare paged forum list model
    /// </summary>
    /// <param name="searchModel">Forum search model</param>
    /// <param name="forumGroup">Forum group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum list model
    /// </returns>
    public async Task<ForumListModel> PrepareForumListModelAsync(ForumSearchModel searchModel, ForumGroup forumGroup)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(forumGroup);

        //get forums
        var forums = (await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = await new ForumListModel().PrepareToGridAsync(searchModel, forums, () =>
        {
            return forums.SelectAwait(async forum =>
            {
                //fill in model values from the entity
                var forumModel = forum.ToModel<ForumModel>();

                //convert dates to the user time
                forumModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forum.CreatedOnUtc, DateTimeKind.Utc);

                return forumModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare forum model
    /// </summary>
    /// <param name="model">Forum model</param>
    /// <param name="forum">Forum</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum model
    /// </returns>
    public async Task<ForumModel> PrepareForumModelAsync(ForumModel model, Forum forum, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (forum != null)
            model ??= forum.ToModel<ForumModel>();

        //set default values for the new model
        if (forum == null)
            model.DisplayOrder = 1;

        //prepare available forum groups
        model.ForumGroups = (await _forumService.GetAllForumGroupsAsync())
            .Select(forumGroup => new SelectListItem(forumGroup.Name, forumGroup.Id.ToString()))
            .ToList();

        return model;
    }

    #endregion
}