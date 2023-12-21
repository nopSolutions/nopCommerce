using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order note list model
/// </summary>
public partial record OrderNoteListModel : BasePagedListModel<OrderNoteModel>
{
}