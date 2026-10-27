namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents a PunchOut setup response
/// </summary>
public class PunchOutSetupResponse : BasePunchOutModel
{
    public string SessionId { get; set; }
    public string StartPageUrl { get; set; }
}
