namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents a PunchOut setup request
/// </summary>
public class PunchOutSetupRequest : BasePunchOutModel
{
    public string Identity { get; set; }
    public string SharedSecret { get; set; }

    public string BuyerCookie { get; set; }
    public string BrowserFormPostUrl { get; set; }

    public PunchOutAddress ShipTo { get; set; }

    public string Contact { get; set; }
}
