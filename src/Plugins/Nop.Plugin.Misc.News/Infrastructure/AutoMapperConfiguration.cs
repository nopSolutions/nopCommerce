using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.News.Admin.Models;
using Nop.Plugin.Misc.News.Domain;

namespace Nop.Plugin.Misc.News.Infrastructure;

/// <summary>
/// Represents mapping configuration for plugin models
/// </summary>
public class AutoMapperConfiguration : Profile, IOrderedMapperProfile
{
    #region Ctor

    public AutoMapperConfiguration()
    {
        //Create news maps
        CreateMap<NewsComment, NewsCommentModel>()
            .ForMember(model => model.CustomerInfo, options => options.Ignore())
            .ForMember(model => model.CreatedOn, options => options.Ignore())
            .ForMember(model => model.CommentText, options => options.Ignore())
            .ForMember(model => model.NewsItemTitle, options => options.Ignore())
            .ForMember(model => model.StoreName, options => options.Ignore());
        CreateMap<NewsCommentModel, NewsComment>()
            .ForMember(entity => entity.CommentTitle, options => options.Ignore())
            .ForMember(entity => entity.CommentText, options => options.Ignore())
            .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
            .ForMember(entity => entity.NewsItemId, options => options.Ignore())
            .ForMember(entity => entity.CustomerId, options => options.Ignore())
            .ForMember(entity => entity.StoreId, options => options.Ignore());

        CreateMap<NewsItem, NewsItemModel>()
            .ForMember(model => model.ApprovedComments, options => options.Ignore())
            .ForMember(model => model.AvailableLanguages, options => options.Ignore())
            .ForMember(model => model.CreatedOn, options => options.Ignore())
            .ForMember(model => model.LanguageName, options => options.Ignore())
            .ForMember(model => model.NotApprovedComments, options => options.Ignore())
            .ForMember(model => model.SeName, options => options.Ignore());
        CreateMap<NewsItemModel, NewsItem>()
            .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

        CreateMap<NewsSettings, ConfigurationModel>()
            .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore())
            .ForMember(model => model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.Enabled_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.MainPageNewsCount_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.NewsArchivePageSize_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.NewsCommentsMustBeApproved_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.NotifyAboutNewNewsComments_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ShowHeaderRssUrl_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ShowNewsOnMainPage_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.ShowCaptchaOnNewsCommentPage_OverrideForStore, options => options.Ignore())
            .ForMember(model => model.SitemapIncludeNews_OverrideForStore, options => options.Ignore());
        CreateMap<ConfigurationModel, NewsSettings>();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Order of this mapper implementation
    /// </summary>
    public int Order => 0;

    #endregion
}
