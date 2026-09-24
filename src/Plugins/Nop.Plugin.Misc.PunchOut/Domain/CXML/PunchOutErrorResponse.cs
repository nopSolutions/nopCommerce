namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents a PunchOut error response
/// </summary>
public class PunchOutErrorResponse : BasePunchOutModel
{
    public string StatusCode { get; set; }

    public string StatusText { get; set; }

    public string ErrorMessage { get; set; }
}
