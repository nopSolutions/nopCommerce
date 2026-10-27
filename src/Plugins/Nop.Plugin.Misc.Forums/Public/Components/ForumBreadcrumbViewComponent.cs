using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Forums.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Forums.Public.Components;

public class ForumBreadcrumbViewComponent : NopViewComponent
{
    #region Fields

    private readonly ForumModelFactory _forumModelFactory;

    #endregion

    #region Ctor

    public ForumBreadcrumbViewComponent(ForumModelFactory forumModelFactory)
    {
        _forumModelFactory = forumModelFactory;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
    {
        var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);

        return View("~/Plugins/Misc.Forums/Public/Views/Components/ForumBreadcrumb/Default.cshtml", model);
    }

    #endregion
}