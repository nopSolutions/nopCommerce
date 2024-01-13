using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Templates;

/// <summary>
/// Represents a manufacturer template list model
/// </summary>
public partial record ManufacturerTemplateListModel : BasePagedListModel<ManufacturerTemplateModel>
{
}