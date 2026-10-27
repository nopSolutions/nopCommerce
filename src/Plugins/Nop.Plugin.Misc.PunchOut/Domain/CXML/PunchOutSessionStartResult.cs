using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents the result of starting a PunchOut session
/// </summary>
public class PunchOutSessionStartResult
{
    public PunchOutSession Session { get; set; }

    public Customer Customer { get; set; }
}
