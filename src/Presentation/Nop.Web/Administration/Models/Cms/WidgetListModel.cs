using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Cms
{
    public class WidgetListModel : BaseNopModel
    {
        public WidgetListModel()
        {
            this.AvailableWidgetPlugins = new List<WidgetPluginModel>();
            this.AvailableWidgets = new List<WidgetModel>();
        }
        public IList<WidgetPluginModel> AvailableWidgetPlugins { get; set; }
        public IList<WidgetModel> AvailableWidgets { get; set; }
    }
}