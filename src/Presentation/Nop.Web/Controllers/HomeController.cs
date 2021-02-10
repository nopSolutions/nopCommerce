using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Fields

        private readonly ITopicModelFactory _topicModelFactory;

        #endregion

        #region Ctor

        public HomeController(ITopicModelFactory topicModelFactory)
        {
            _topicModelFactory = topicModelFactory;
        }

        #endregion

        #region Methods

        public virtual async System.Threading.Tasks.Task<IActionResult> IndexAsync()
        {
            var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync("HomepageText");

            return View(model);
        }

        #endregion
    }
}