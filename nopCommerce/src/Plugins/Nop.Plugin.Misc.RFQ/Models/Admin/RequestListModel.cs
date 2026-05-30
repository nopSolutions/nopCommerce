using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a request a quote list model
/// </summary>
public record RequestListModel : BasePagedListModel<RequestQuoteModel>;