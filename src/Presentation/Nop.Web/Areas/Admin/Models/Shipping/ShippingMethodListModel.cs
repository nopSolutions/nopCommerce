using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping method list model
    /// </summary>
    public partial record ShippingMethodListModel : BasePagedListModel<ShippingMethodModel>
    {
    }
}