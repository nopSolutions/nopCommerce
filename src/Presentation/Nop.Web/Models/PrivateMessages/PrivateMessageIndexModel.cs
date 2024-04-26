using Nop.Web.Framework.Models;

namespace Nop.Web.Models.PrivateMessages;

public partial record PrivateMessageIndexModel : BaseNopModel
{
    public int InboxPage { get; set; }
    public int SentItemsPage { get; set; }
    public bool SentItemsTabSelected { get; set; }
}