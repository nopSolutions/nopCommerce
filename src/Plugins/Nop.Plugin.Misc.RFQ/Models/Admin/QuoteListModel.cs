using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents the quote list model
/// </summary>
public record QuoteListModel : BasePagedListModel<QuoteModel>
{
}