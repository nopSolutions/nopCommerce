using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public class AdminTabStripCreated
    {
        public AdminTabStripCreated(HtmlHelper helper, string tabStripName)
        {
            this.Helper = helper;
            this.TabStripName = tabStripName;
            this.BlocksToRender = new List<MvcHtmlString>();
        }

        public HtmlHelper Helper { get; private set; }
        public string TabStripName { get; private set; }
        public IList<MvcHtmlString> BlocksToRender { get; set; }
    }
}
