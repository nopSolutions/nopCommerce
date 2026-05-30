using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.Forums.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Admin.CONFIGURATION,
            pattern: "Admin/Forum/Configure",
            defaults: new { controller = "Forum", action = "Configuration", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.BOARDS,
            pattern: $"{lang}/boards",
            defaults: new { controller = "Boards", action = "Index" });

        //forums
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.ACTIVE_DISCUSSIONS,
            pattern: $"{lang}/boards/activediscussions",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.ACTIVE_DISCUSSIONS_PAGED,
            pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.ACTIVE_DISCUSSIONS_RSS,
            pattern: $"boards/activediscussionsrss",
            defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.POST_EDIT,
            pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostEdit" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.POST_DELETE,
            pattern: $"{lang}/boards/postdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "PostDelete" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.POST_CREATE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.POST_CREATE_QUOTE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_EDIT,
            pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicEdit" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_DELETE,
            pattern: $"{lang}/boards/topicdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "TopicDelete" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_CREATE,
            pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicCreate" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_MOVE,
            pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicMove" });

        //topic watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_WATCH,
            pattern: $"boards/topicwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicWatch" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_SLUG,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Topic" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.TOPIC_SLUG_PAGED,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Topic" });

        //forum watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_WATCH,
            pattern: $"boards/forumwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumWatch" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_RSS,
            pattern: $"boards/forumrss/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumRSS" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_SLUG,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_SLUG_PAGED,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_GROUP_SLUG,
            pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "ForumGroup" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.BOARDS_SEARCH,
            pattern: $"{lang}/boards/search",
            defaults: new { controller = "Boards", action = "Search" });

        //post vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.POST_VOTE,
            pattern: "boards/postvote",
            defaults: new { controller = "Boards", action = "PostVote" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.CUSTOMER_FORUM_SUBSCRIPTIONS,
            pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "CustomerForumSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_PROFILE_POSTS,
            pattern: $"{lang}/profile/{{id:min(0)}}/posts",
            defaults: new { controller = "Boards", action = "ProfilePosts" });

        endpointRouteBuilder.MapControllerRoute(name: ForumDefaults.Routes.Public.FORUM_PROFILE_POSTS_PAGED,
            pattern: $"{lang}/profile/{{id:min(0)}}/posts/page/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "ProfilePosts" });

    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}