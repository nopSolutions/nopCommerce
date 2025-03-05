using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.MercadoPagoPlugin.Components;
[ViewComponent(Name = "Custom")]
public class CustomViewComponent : NopViewComponent
{
    public CustomViewComponent()
    {

    }

    public IViewComponentResult Invoke(int productId)
    {
        throw new NotImplementedException();
    }
}
