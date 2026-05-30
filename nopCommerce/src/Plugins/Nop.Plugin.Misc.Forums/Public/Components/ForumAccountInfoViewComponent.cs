using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Forums.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Forums.Public.Components;

public class ForumAccountInfoViewComponent : NopViewComponent
{
    #region Fields

    private readonly ForumModelFactory _forumModelFactory;
    private readonly ForumSettings _forumSettings;

    #endregion

    #region Ctor

    public ForumAccountInfoViewComponent(ForumModelFactory forumModelFactory, ForumSettings forumSettings)
    {
        _forumModelFactory = forumModelFactory;
        _forumSettings = forumSettings;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_forumSettings.ForumsEnabled || !_forumSettings.SignaturesEnabled)
            return Content("");

        var model = await _forumModelFactory.PrepareForumAccountInfoModelAsync();

        return View("~/Plugins/Misc.Forums/Public/Views/Components/ForumAccountInfo/Default.cshtml", model);
    }

    #endregion
}
