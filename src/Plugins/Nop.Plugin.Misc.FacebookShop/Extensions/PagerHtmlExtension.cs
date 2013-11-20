using System.Web.Mvc;
using Nop.Web.Framework.UI.Paging;

namespace Nop.Plugin.Misc.FacebookShop.Extensions
{
    public static class PagerHtmlExtension
    {
        //jsut a copy of \Presentation\Nop.Web\Extensions\PagerHtmlExtension.cs
        public static Pager Pager(this HtmlHelper helper, IPageableModel pagination)
        {
            return new Pager(pagination, helper.ViewContext);
        }
    }
}
