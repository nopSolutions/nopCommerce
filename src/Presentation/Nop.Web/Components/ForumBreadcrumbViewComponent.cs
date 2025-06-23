using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ForumBreadcrumbViewComponent : NopViewComponent
{
    protected readonly IForumModelFactory _forumModelFactory;

    public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
    {
        _forumModelFactory = forumModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="forumGroupId">The forum group identifier</param>
    /// <param name="forumId">The forum identifier</param>
    /// <param name="forumTopicId">The forum topic identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
    {
        var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);
        return await ViewAsync(model);
    }
}