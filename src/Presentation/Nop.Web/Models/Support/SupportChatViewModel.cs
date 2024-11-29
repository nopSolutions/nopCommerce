using Nop.Core.Domain.Support;

namespace Nop.Web.Models.Support;

public class SupportChatViewModel
{
    public int RequestId { get; set; }
    public string Subject { get; set; }
    public StatusEnum Status { get; set; }
    public List<SupportMessageModel> Messages { get; set; }
    public string NewMessage { get; set; }
}