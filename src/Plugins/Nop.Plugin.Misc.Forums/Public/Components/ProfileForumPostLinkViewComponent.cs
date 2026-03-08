using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Profile;

namespace Nop.Plugin.Misc.Forums.Public.Components;

public class ProfileForumPostLinkViewComponent : NopViewComponent
{
    #region Fields

    private readonly ForumSettings _forumSettings;

    #endregion

    #region Ctor

    public ProfileForumPostLinkViewComponent(ForumSettings forumSettings)
    {
        _forumSettings = forumSettings;
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        if (!_forumSettings.ForumsEnabled || !widgetZone.Equals(PublicWidgetZones.ProfilePageInfoUserstats, StringComparison.OrdinalIgnoreCase))
            return Content("");

        if (additionalData is not ProfileInfoModel profileInfo)
            return Content("");

        return View("~/Plugins/Misc.Forums/Public/Views/Components/ProfileForumPostLink/Default.cshtml", profileInfo);
    }

    #endregion
}