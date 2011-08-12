using System.Web.Mvc;

namespace Nop.Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "Customer", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Login", "Customer");
        }

        public static string LogOff(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Logout", "Customer", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Logout", "Customer");
        }
    }
}
