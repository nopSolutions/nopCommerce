using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class ForumLastPostViewComponent : ViewComponent
    {
        private readonly IForumService _forumService;
        private readonly IForumModelFactory _forumModelFactory;

        public ForumLastPostViewComponent(IForumService forumService, IForumModelFactory forumModelFactory)
        {
            this._forumService = forumService;
            this._forumModelFactory = forumModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
        {
            var forumPost = _forumService.GetPostById(forumPostId);
            var model = _forumModelFactory.PrepareLastPostModel(forumPost, showTopic);

            return View(model);
        }
    }
}
