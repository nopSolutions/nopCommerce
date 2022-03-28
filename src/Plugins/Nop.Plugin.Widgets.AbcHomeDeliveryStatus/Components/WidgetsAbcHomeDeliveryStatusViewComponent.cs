using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Models;
using Nop.Web.Framework.Components;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Components
{
    [ViewComponent(Name = "WidgetsAbcHomeDeliveryStatus")]
    public class WidgetsAbcHomeDeliveryStatusViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            var topicSystemName = (string)additionalData;
            if (topicSystemName == "home-delivery-status")
                return View("~/Plugins/Widgets.AbcHomeDeliveryStatus/Views/HomeDeliveryStatus.cshtml", new HomeDeliveryStatusModel());
            return Content("");
        }
    }
}
