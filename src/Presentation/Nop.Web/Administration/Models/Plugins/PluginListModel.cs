using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Plugins
{
    public partial class PluginListModel : BaseNopModel
    {
        public PluginListModel()
        {
            AvailableLoadModes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
        public int SearchLoadModeId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
        public IList<SelectListItem> AvailableLoadModes { get; set; }
    }
}