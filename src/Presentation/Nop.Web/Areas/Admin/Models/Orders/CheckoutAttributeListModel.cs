using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a checkout attribute list model
    /// </summary>
    public record CheckoutAttributeListModel : BasePagedListModel<CheckoutAttributeModel>
    {
    }
}