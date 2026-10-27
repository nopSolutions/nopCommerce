using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.PunchOut.Models.Session;

/// <summary>
/// Represents PunchOutSession search model
/// </summary>
public record PunchOutSessionModel : BaseNopModel
{
    public string SessionId { get; set; }
    public string BuyerCookie { get; set; }
    public bool IsActive { get; set; }
    public int CustomerId { get; set; }
    public string CustomerEmail { get; set; }
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
