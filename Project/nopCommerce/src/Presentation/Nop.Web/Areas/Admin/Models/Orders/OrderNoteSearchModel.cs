using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order note search model
/// </summary>
public partial record OrderNoteSearchModel : BaseSearchModel
{
    #region Properties

    public int OrderId { get; set; }

    #endregion
}