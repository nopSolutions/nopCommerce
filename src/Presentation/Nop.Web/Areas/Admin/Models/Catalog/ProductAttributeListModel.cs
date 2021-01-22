using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute list model
    /// </summary>
    public partial record ProductAttributeListModel : BasePagedListModel<ProductAttributeModel>
    {
    }
}