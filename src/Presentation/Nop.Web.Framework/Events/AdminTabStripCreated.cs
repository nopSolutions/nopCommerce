
using Telerik.Web.Mvc.UI;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Admin tabstrip created event
    /// </summary>
    public class AdminTabStripCreated
    {
        public AdminTabStripCreated(TabStripItemFactory itemFactory, string tabStripName)
        {
            this.ItemFactory = itemFactory;
            this.TabStripName = tabStripName;
        }

        public TabStripItemFactory ItemFactory { get; private set; }
        public string TabStripName { get; private set; }
    }
}
