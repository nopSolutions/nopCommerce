using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Brevo.Models;

/// <summary>
/// Represents message template list model
/// </summary>
public record BrevoMessageTemplateListModel : BasePagedListModel<BrevoMessageTemplateModel>
{
}