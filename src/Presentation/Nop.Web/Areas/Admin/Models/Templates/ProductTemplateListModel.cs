using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a product template list model
/// </summary>
public partial record ProductTemplateListModel : BasePagedListModel<ProductTemplateModel>
{
}