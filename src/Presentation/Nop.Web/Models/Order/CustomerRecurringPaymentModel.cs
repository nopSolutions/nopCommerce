using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order;

public partial record CustomerRecurringPaymentModel : BaseNopEntityModel
{
    public string StartDate { get; set; }
    public string CycleInfo { get; set; }
    public string NextPayment { get; set; }
    public int TotalCycles { get; set; }
    public int CyclesRemaining { get; set; }
    public int InitialOrderId { get; set; }
    public bool CanRetryLastPayment { get; set; }
    public string InitialOrderNumber { get; set; }
    public bool CanCancel { get; set; }
}
