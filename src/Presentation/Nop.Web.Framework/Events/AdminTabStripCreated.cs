using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public class AdminTabStripCreated
    {
#if NET451
        public AdminTabStripCreated(HtmlHelper helper, string tabStripName)
        {
            this.Helper = helper;
            this.TabStripName = tabStripName;
            this.BlocksToRender = new List<MvcHtmlString>();
        }

        public HtmlHelper Helper { get; private set; }
        public string TabStripName { get; private set; }
        public IList<MvcHtmlString> BlocksToRender { get; set; }
#endif
    }
}
