using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class ForumActiveDiscussionsSmallViewComponent : ViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;

        public ForumActiveDiscussionsSmallViewComponent(IForumModelFactory forumModelFactory)
        {
            this._forumModelFactory = forumModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _forumModelFactory.PrepareActiveDiscussionsModel();
            if (!model.ForumTopics.Any())
                return Content("");

            return View(model);
        }
    }
}
