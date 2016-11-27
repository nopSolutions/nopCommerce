using System.Collections.Generic;
using Nop.Web.Models.Cms;

namespace Nop.Web.Factories
{
    public partial interface IWidgetModelFactory
    {
        List<RenderWidgetModel> GetRenderWidgetModels(string widgetZone, object additionalData = null);
    }
}
