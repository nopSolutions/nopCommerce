using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer role product list model
/// </summary>
public partial record CustomerRoleProductListModel : BasePagedListModel<ProductModel>
{
}