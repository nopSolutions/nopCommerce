namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class ThreeDS
{
#if DEBUG
    public string Type { get; set; } = "disabled";
#else
    public string Type { get; set; } = "enabled";
#endif
}
