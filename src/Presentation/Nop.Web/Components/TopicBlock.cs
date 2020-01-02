using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class TopicBlockViewComponent : NopViewComponent
    {
        private readonly ITopicModelFactory _topicModelFactory;

        public TopicBlockViewComponent(ITopicModelFactory topicModelFactory)
        {
            _topicModelFactory = topicModelFactory;
        }

        public IViewComponentResult Invoke(string systemName)
        {
            var model = _topicModelFactory.PrepareTopicModelBySystemName(systemName);
            if (model == null)
                return Content("");
            return View(model);
        }
    }
}
