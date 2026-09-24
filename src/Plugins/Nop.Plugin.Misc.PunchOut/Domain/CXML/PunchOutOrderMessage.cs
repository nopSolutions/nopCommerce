namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents a PunchOut order message
/// </summary>
public class PunchOutOrderMessage : BasePunchOutModel
{
    public string BuyerCookie { get; set; }

    public decimal Total { get; set; }

    public IList<PunchOutOrderItem> Items { get; set; }
        = new List<PunchOutOrderItem>();
}
