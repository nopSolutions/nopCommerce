using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Page rendering event
    /// </summary>
    public class PageRenderingEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="helper">HTML Helper</param>
        public PageRenderingEvent(IHtmlHelper helper)
        {
            this.Helper = helper;
        }

        /// <summary>
        /// HTML helper
        /// </summary>
        public IHtmlHelper Helper { get; private set; }
    }
}
