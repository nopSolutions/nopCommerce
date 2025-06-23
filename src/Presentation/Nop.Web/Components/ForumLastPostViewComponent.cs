using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ForumLastPostViewComponent : NopViewComponent
{
    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly IForumService _forumService;

    public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService)
    {
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="forumPostId">The forum post identifier</param>
    /// <param name="showTopic">Whether to show topic</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
    {
        var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
        var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);

        return await ViewAsync(model);
    }
}