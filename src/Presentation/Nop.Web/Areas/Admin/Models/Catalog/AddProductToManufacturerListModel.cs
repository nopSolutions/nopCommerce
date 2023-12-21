using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product list model to add to the manufacturer
/// </summary>
public partial record AddProductToManufacturerListModel : BasePagedListModel<ProductModel>
{
}