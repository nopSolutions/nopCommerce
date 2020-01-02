using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ForumBreadcrumbViewComponent : NopViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;

        public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
        {
            _forumModelFactory = forumModelFactory;
        }

        public IViewComponentResult Invoke(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = _forumModelFactory.PrepareForumBreadcrumbModel(forumGroupId, forumId, forumTopicId);
            return View(model);
        }
    }
}
