using System.Linq;
using System.Web.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    /// <summary>
    /// 页面部件
    /// </summary>
    public partial class WidgetController : BasePublicController
    {
		#region Fields

        private readonly IWidgetModelFactory _widgetModelFactory;

        #endregion

        #region Ctor

        public WidgetController(IWidgetModelFactory widgetModelFactory)
        {
            this._widgetModelFactory = widgetModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 页面部件
        /// </summary>
        /// <param name="widgetZone"></param>
        /// <param name="additionalData"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public virtual ActionResult WidgetsByZone(string widgetZone, object additionalData = null)
        {
            var model = _widgetModelFactory.GetRenderWidgetModels(widgetZone, additionalData);

            //no data?
            //如果没有数据，返回为空白字符串
            if (!model.Any())
                return Content("");

            return PartialView(model);
        }

        #endregion
    }
}
