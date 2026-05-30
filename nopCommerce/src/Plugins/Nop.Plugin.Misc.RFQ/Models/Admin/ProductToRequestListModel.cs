using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a product list model to add to the quote
/// </summary>
public record ProductToRequestListModel : BasePagedListModel<ProductModel>;