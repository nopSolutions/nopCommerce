using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.PrivateMessages;

public partial record PrivateMessageListModel : BaseNopModel
{
    public IList<PrivateMessageModel> Messages { get; set; }
    public PagerModel PagerModel { get; set; }
}