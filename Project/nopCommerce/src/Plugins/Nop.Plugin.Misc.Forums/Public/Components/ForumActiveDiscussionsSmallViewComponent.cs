using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Forums.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Forums.Public.Components;

public class ForumActiveDiscussionsSmallViewComponent : NopViewComponent
{
    #region Fields

    private readonly ForumModelFactory _forumModelFactory;

    #endregion

    #region Ctor

    public ForumActiveDiscussionsSmallViewComponent(ForumModelFactory forumModelFactory)
    {
        _forumModelFactory = forumModelFactory;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();
        if (!model.ForumTopics.Any())
            return Content("");

        return View("~/Plugins/Misc.Forums/Public/Views/Components/ForumActiveDiscussionsSmall/Default.cshtml", model);
    }

    #endregion
}