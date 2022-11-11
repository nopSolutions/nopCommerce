using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a manufacturer list model to add to the discount
    /// </summary>
    public partial record AddManufacturerToDiscountListModel : BasePagedListModel<ManufacturerModel>
    {
    }
}