using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ForumLastPostViewComponent : NopViewComponent
    {
        private readonly IForumService _forumService;
        private readonly IForumModelFactory _forumModelFactory;

        public ForumLastPostViewComponent(IForumService forumService, IForumModelFactory forumModelFactory)
        {
            this._forumService = forumService;
            this._forumModelFactory = forumModelFactory;
        }

        public IViewComponentResult Invoke(int forumPostId, bool showTopic)
        {
            var forumPost = _forumService.GetPostById(forumPostId);
            var model = _forumModelFactory.PrepareLastPostModel(forumPost, showTopic);

            return View(model);
        }
    }
}
