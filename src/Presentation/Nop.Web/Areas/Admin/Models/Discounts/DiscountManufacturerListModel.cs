using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount manufacturer list model
    /// </summary>
    public partial record DiscountManufacturerListModel : BasePagedListModel<DiscountManufacturerModel>
    {
    }
}