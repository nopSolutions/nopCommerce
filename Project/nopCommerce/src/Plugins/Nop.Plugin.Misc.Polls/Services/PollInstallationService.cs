using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.Polls.Services;

/// <summary>
/// Plugin installation service
/// </summary>
public class PollInstallationService
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IRepository<PermissionRecord> _permissionRepository;
    private readonly IRepository<PermissionRecordCustomerRoleMapping> _permissionMappingRepository;
    private readonly ISettingService _settingService;
    private readonly IWorkContext _workContext;
    private readonly PollService _pollService;

    #endregion

    #region Ctor

    public PollInstallationService(ILocalizationService localizationService,
        IRepository<PermissionRecord> permissionRepository,
        IRepository<PermissionRecordCustomerRoleMapping> permissionMappingRepository,
        ISettingService settingService,
        IWorkContext workContext,
        PollService pollService)
    {
        _localizationService = localizationService;
        _permissionRepository = permissionRepository;
        _permissionMappingRepository = permissionMappingRepository;
        _settingService = settingService;
        _workContext = workContext;
        _pollService = pollService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create permission records (if it doesn't exist)
    /// </summary>
    /// <param name="oldSystemName">Old name of the permission record</param>
    /// <param name="newSystemName">New name of the permission record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task UpdatePermissionMappingsAsync(string oldSystemName, string newSystemName)
    {
        ArgumentException.ThrowIfNullOrEmpty(oldSystemName);
        ArgumentException.ThrowIfNullOrEmpty(newSystemName);

        if (_permissionRepository.Table.FirstOrDefault(pr => pr.SystemName == oldSystemName) is PermissionRecord oldRec)
        {
            var roleIds = _permissionMappingRepository.Table
                .Where(pm => pm.PermissionRecordId == oldRec.Id)
                .Select(pm => pm.CustomerRoleId)
                .ToArray();

            if (roleIds.Any() && _permissionRepository.Table.FirstOrDefault(pr => pr.SystemName == newSystemName) is PermissionRecord newRec)
            {
                foreach (var roleId in roleIds)
                {
                    try
                    {
                        await _permissionMappingRepository.InsertAsync(new PermissionRecordCustomerRoleMapping
                        {
                            CustomerRoleId = roleId,
                            PermissionRecordId = newRec.Id
                        });
                    }
                    catch
                    {
                        //exist
                    }
                }

                await _permissionMappingRepository.DeleteAsync(pr => pr.PermissionRecordId == oldRec.Id);
                await _permissionRepository.DeleteAsync(pr => pr.Id == oldRec.Id);
            }
        }
    }

    /// <summary>
    /// Initialize <see cref="Poll.ShowInLeftSideColumn" /> based on <see cref="Poll.SystemKeyword" /> to preserve the previous behavior
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task MapExistedPollsInLeftSidebarAsync()
    {
        var existedPolls = await _pollService.GetPollsAsync(systemKeyword: "LeftColumnPoll");
        if (!existedPolls.Any())
            return;

        foreach (var poll in existedPolls)
        {
            poll.ShowInLeftSideColumn = true;
            await _pollService.UpdatePollAsync(poll);
        }
    }

    /// <summary>
    /// Update permission record names
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task PreparePermissionMappingsAsync()
    {
        await UpdatePermissionMappingsAsync("ContentManagement.PollsView", PollsDefaults.Permissions.POLLS_VIEW);
        await UpdatePermissionMappingsAsync("ContentManagement.PollsCreateEditDelete", PollsDefaults.Permissions.POLLS_MANAGE);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Install sample polls
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallSampleDataAsync()
    {
        var language = await _workContext.GetWorkingLanguageAsync();

        var poll = new Poll
        {
            LanguageId = language.Id,
            Name = "Do you like nopCommerce?",
            Published = true,
            ShowOnHomepage = true,
            ShowInLeftSideColumn = true,
            DisplayOrder = 1
        };

        await _pollService.InsertPollAsync(poll);

        await _pollService.InsertPollAnswerAsync(new()
        {
            PollId = poll.Id,
            Name = "Excellent",
            DisplayOrder = 1
        });

        await _pollService.InsertPollAnswerAsync(new()
        {
            PollId = poll.Id,
            Name = "Good",
            DisplayOrder = 2
        });

        await _pollService.InsertPollAnswerAsync(new()
        {
            PollId = poll.Id,
            Name = "Poor",
            DisplayOrder = 3
        });

        await _pollService.InsertPollAnswerAsync(new()
        {
            PollId = poll.Id,
            Name = "Very bad",
            DisplayOrder = 4
        });
    }

    /// <summary>
    /// Adds the necessary data for the plugin to work correctly
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InstallRequiredDataAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new PollSettings
        {
            Enabled = true
        });

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.Polls"] = "Polls",
            ["Plugins.Misc.Polls.Added"] = "The new poll has been added successfully.",
            ["Plugins.Misc.Polls.AddNew"] = "Add a new poll",
            ["Plugins.Misc.Polls.Answers"] = "Poll answers",
            ["Plugins.Misc.Polls.Answers.Fields.DisplayOrder"] = "Display order",
            ["Plugins.Misc.Polls.Answers.Fields.DisplayOrder.Hint"] = "The display order of this poll answer. 1 represents the top of the list.",
            ["Plugins.Misc.Polls.Answers.Fields.Name"] = "Name",
            ["Plugins.Misc.Polls.Answers.Fields.Name.Hint"] = "Enter the poll answer name.",
            ["Plugins.Misc.Polls.Answers.Fields.Name.Required"] = "Name is required.",
            ["Plugins.Misc.Polls.Answers.Fields.NumberOfVotes"] = "Number of votes",
            ["Plugins.Misc.Polls.Answers.SaveBeforeEdit"] = "You need to save the poll before you can add answers for this poll page.",
            ["Plugins.Misc.Polls.BackToList"] = "back to poll list",
            ["Plugins.Misc.Polls.Configuration.Enabled"] = "Enabled",
            ["Plugins.Misc.Polls.Configuration.Enabled.Hint"] = "Check to enable the polls in your store.",
            ["Plugins.Misc.Polls.Deleted"] = "The poll has been deleted successfully.",
            ["Plugins.Misc.Polls.EditPollDetails"] = "Edit poll details",
            ["Plugins.Misc.Polls.Fields.AllowGuestsToVote"] = "Allow guests to vote",
            ["Plugins.Misc.Polls.Fields.AllowGuestsToVote.Hint"] = "Check to allow guests to vote.",
            ["Plugins.Misc.Polls.Fields.DisplayOrder"] = "Display order",
            ["Plugins.Misc.Polls.Fields.DisplayOrder.Hint"] = "The display order of this poll. 1 represents the top of the list.",
            ["Plugins.Misc.Polls.Fields.EndDate"] = "End date",
            ["Plugins.Misc.Polls.Fields.EndDate.Hint"] = "Set the poll end date in Coordinated Universal Time (UTC). You can also leave it empty.",
            ["Plugins.Misc.Polls.Fields.Language"] = "Language",
            ["Plugins.Misc.Polls.Fields.Language.Hint"] = "The language of this poll. A customer will only see the polls for their selected language.",
            ["Plugins.Misc.Polls.Fields.LimitedToStores"] = "Limited to stores",
            ["Plugins.Misc.Polls.Fields.LimitedToStores.Hint"] = "Option to limit this poll to a certain store. If you have multiple stores, choose one or several from the list. If you don't use this option just leave this field empty.",
            ["Plugins.Misc.Polls.Fields.Name"] = "Name",
            ["Plugins.Misc.Polls.Fields.Name.Hint"] = "The name of this poll.",
            ["Plugins.Misc.Polls.Fields.Name.Required"] = "Name is required.",
            ["Plugins.Misc.Polls.Fields.Published"] = "Published",
            ["Plugins.Misc.Polls.Fields.Published.Hint"] = "Determines whether this poll is published (visible) in your store.",
            ["Plugins.Misc.Polls.Fields.ShowOnHomepage"] = "Show on home page",
            ["Plugins.Misc.Polls.Fields.ShowOnHomepage.Hint"] = "Check if you want to show poll on the home page.",
            ["Plugins.Misc.Polls.Fields.ShowInLeftSideColumn"] = "Show in the left-side columne",
            ["Plugins.Misc.Polls.Fields.ShowInLeftSideColumn.Hint"] = "Check if you want to show poll on the left-side column.",
            ["Plugins.Misc.Polls.Fields.StartDate"] = "Start date",
            ["Plugins.Misc.Polls.Fields.StartDate.Hint"] = "Set the poll start date in Coordinated Universal Time (UTC). You can also leave it empty.",
            ["Plugins.Misc.Polls.Info"] = "Poll Info",
            ["Plugins.Misc.Polls.List.SearchStore"] = "Store",
            ["Plugins.Misc.Polls.List.SearchStore.Hint"] = "Search by a specific store.",
            ["Plugins.Misc.Polls.Updated"] = "The poll has been updated successfully.",
            ["Plugins.Misc.Polls.Documentation.Reference"] = "Learn more about <a target=\"_blank\" href=\"{0}\">polls</a>",
            ["Plugins.Misc.Polls.OnlyRegisteredUsersVote"] = "Only registered users can vote.",
            ["Plugins.Misc.Polls.SelectAnswer"] = "Please select an answer",
            ["Plugins.Misc.Polls.Title"] = "Community poll",
            ["Plugins.Misc.Polls.TotalVotes"] = "{0} vote(s)...",
            ["Plugins.Misc.Polls.Vote"] = "Vote",
            ["Plugins.Misc.Polls.VotesResultLine"] = "{0} ({1} vote(s) - {2}%)",
            ["Security.Permission.Polls.Manage"] = "Admin area. Polls. Create, edit, delete",
            ["Security.Permission.Polls.View"] = "Admin area. Polls. View",
        });

        await PreparePermissionMappingsAsync();

        await MapExistedPollsInLeftSidebarAsync();
    }

    /// <summary>
    /// Removes the data inserted in <see cref="InstallRequiredDataAsync"/>
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UnInstallRequiredDataAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<PollSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Polls");
        await _localizationService.DeleteLocaleResourcesAsync("Security.Permission.Polls");

        //permission
        await _permissionRepository.DeleteAsync(record => record.SystemName == PollsDefaults.Permissions.POLLS_MANAGE
            || record.SystemName == PollsDefaults.Permissions.POLLS_VIEW);
    }

    #endregion
}