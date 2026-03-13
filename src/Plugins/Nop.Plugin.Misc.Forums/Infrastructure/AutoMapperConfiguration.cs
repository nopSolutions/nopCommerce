using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.Forums.Admin.Models;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Infrastructure;

/// <summary>
/// Represents mapping configuration for plugin models
/// </summary>
public class AutoMapperConfiguration : Profile, IOrderedMapperProfile
{
    #region Ctor

    public AutoMapperConfiguration()
    {
        //Create forum maps
        CreateMap<Forum, ForumModel>()
            .ForMember(model => model.CreatedOn, options => options.Ignore())
            .ForMember(model => model.ForumGroups, options => options.Ignore());
        CreateMap<ForumModel, Forum>()
            .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
            .ForMember(entity => entity.LastPostCustomerId, options => options.Ignore())
            .ForMember(entity => entity.LastPostId, options => options.Ignore())
            .ForMember(entity => entity.LastPostTime, options => options.Ignore())
            .ForMember(entity => entity.LastTopicId, options => options.Ignore())
            .ForMember(entity => entity.NumPosts, options => options.Ignore())
            .ForMember(entity => entity.NumTopics, options => options.Ignore())
            .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

        CreateMap<ForumGroup, ForumGroupModel>()
            .ForMember(model => model.CreatedOn, options => options.Ignore());
        CreateMap<ForumGroupModel, ForumGroup>()
            .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
            .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());

        CreateMap<ForumSettings, ConfigurationModel>()
            .ForMember(model => model.ActiveDiscussionsFeedCount_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ActiveDiscussionsFeedEnabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ActiveDiscussionsPageSize_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowCustomersToDeletePosts_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowCustomersToEditPosts_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowCustomersToManageSubscriptions_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowGuestsToCreatePosts_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowGuestsToCreateTopics_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.AllowPostVoting_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ForumEditorValues, options => options.Ignore())
            .ForMember(model => model.ForumEditor_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ForumFeedCount_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ForumFeedsEnabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ForumsEnabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.MaxVotesPerDay_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.PostsPageSize_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.RelativeDateTimeFormattingEnabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.SearchResultsPageSize_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ShowCustomersPostCount_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.SignaturesEnabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.TopicsPageSize_OverrideForStore, options => options.Ignore());
        CreateMap<ConfigurationModel, ForumSettings>()
            .ForMember(settings => settings.ForumSearchTermMinimumLength, options => options.Ignore())
            .ForMember(settings => settings.ForumSubscriptionsPageSize, options => options.Ignore())
            .ForMember(settings => settings.HomepageActiveDiscussionsTopicCount, options => options.Ignore())
            .ForMember(settings => settings.LatestCustomerPostsPageSize, options => options.Ignore())
            .ForMember(settings => settings.PostMaxLength, options => options.Ignore())
            .ForMember(settings => settings.StrippedTopicMaxLength, options => options.Ignore())
            .ForMember(settings => settings.TopicSubjectMaxLength, options => options.Ignore())
            .ForMember(settings => settings.TopicMetaDescriptionLength, options => options.Ignore())
            .ForMember(settings => settings.BbcodeEditorOpenLinksInNewWindow, options => options.Ignore());
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 0;

    #endregion
}