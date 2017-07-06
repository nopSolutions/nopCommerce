using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class ForumBreadcrumbViewComponent : ViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;

        public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
        {
            this._forumModelFactory = forumModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = _forumModelFactory.PrepareForumBreadcrumbModel(forumGroupId, forumId, forumTopicId);
            return View(model);
        }
    }
}
