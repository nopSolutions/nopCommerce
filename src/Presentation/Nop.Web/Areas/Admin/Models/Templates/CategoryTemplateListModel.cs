using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a category template list model
/// </summary>
public partial record CategoryTemplateListModel : BasePagedListModel<CategoryTemplateModel>
{
}