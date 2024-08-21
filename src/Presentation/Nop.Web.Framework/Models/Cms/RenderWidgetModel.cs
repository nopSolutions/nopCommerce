namespace Nop.Web.Framework.Models.Cms;

public partial record RenderWidgetModel : BaseNopModel
{
    public Type WidgetViewComponent { get; set; }
    public object WidgetViewComponentArguments { get; set; }
}