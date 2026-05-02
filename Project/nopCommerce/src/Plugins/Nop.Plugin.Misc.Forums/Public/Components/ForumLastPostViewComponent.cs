using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Forums.Public.Factories;
using Nop.Plugin.Misc.Forums.Services;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Forums.Public.Components;

public class ForumLastPostViewComponent : NopViewComponent
{
    #region Fields

    private readonly ForumModelFactory _forumModelFactory;
    private readonly ForumService _forumService;

    #endregion

    #region Ctor

    public ForumLastPostViewComponent(ForumModelFactory forumModelFactory, ForumService forumService)
    {
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
    {
        var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
        var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);

        return View("~/Plugins/Misc.Forums/Public/Views/Components/ForumLastPost/Default.cshtml", model);
    }

    #endregion
}