using System.Web.Mvc;

namespace Nop.Web.Framework.Themes
{
    public class MobileCapableRazorViewEngine : ThemableRazorViewEngine
    {
        public string MobileViewModifier { get; set; }

        public MobileCapableRazorViewEngine()
            : this("Mobile")
        {
        }

        public MobileCapableRazorViewEngine(string mobileViewModifier)
        {
            this.MobileViewModifier = mobileViewModifier;
        }

        protected virtual bool IsMobileDevice(ControllerContext controllerContext)
        {
            return controllerContext.HttpContext.Request.Browser.IsMobileDevice;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, 
            string viewName, string masterName, bool useCache)
        {
            bool isMobileDevice = IsMobileDevice(controllerContext);
            string overrideViewName = isMobileDevice ?
                string.Format("{0}.{1}", viewName, MobileViewModifier)
                : viewName;

            ViewEngineResult result = base.FindView(controllerContext, overrideViewName, masterName, useCache);
            // If we're looking for a Mobile view and couldn't find it try again without modifying the viewname
            if (isMobileDevice && (result == null || result.View == null))
                result = base.FindView(controllerContext, viewName, masterName, useCache);
            return result;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, 
            string partialViewName, bool useCache)
        {
            bool isMobileDevice = IsMobileDevice(controllerContext);
            string overrideViewName = isMobileDevice ?
                string.Format("{0}.{1}", partialViewName, MobileViewModifier)
                : partialViewName;

            ViewEngineResult result = base.FindPartialView(controllerContext, overrideViewName, useCache);
            // If we're looking for a Mobile view and couldn't find it try again without modifying the viewname
            if (isMobileDevice && (result == null || result.View == null))
                result = base.FindPartialView(controllerContext, partialViewName, useCache);
            return result;
        }
    }
}
