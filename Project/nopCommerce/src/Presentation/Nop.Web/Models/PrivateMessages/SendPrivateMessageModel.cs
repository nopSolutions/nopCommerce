using Nop.Web.Framework.Models;

namespace Nop.Web.Models.PrivateMessages;

public partial record SendPrivateMessageModel : BaseNopEntityModel
{
    public int ToCustomerId { get; set; }
    public string CustomerToName { get; set; }
    public bool AllowViewingToProfile { get; set; }

    public int ReplyToMessageId { get; set; }

    public string Subject { get; set; }

    public string Message { get; set; }
}