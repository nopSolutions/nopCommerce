using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ForumLastPostViewComponent : NopViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;
        private readonly IForumService _forumService;

        public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService)
        {
            _forumModelFactory = forumModelFactory;
            _forumService = forumService;
        }

        public IViewComponentResult Invoke(int forumPostId, bool showTopic)
        {
            var forumPost = _forumService.GetPostById(forumPostId);
            var model = _forumModelFactory.PrepareLastPostModel(forumPost, showTopic);

            return View(model);
        }
    }
}
