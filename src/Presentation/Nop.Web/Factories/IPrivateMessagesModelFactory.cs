using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Factories
{
    public partial interface IPrivateMessagesModelFactory
    {
        PrivateMessageIndexModel PreparePrivateMessageIndexModel(int? page, string tab);

        PrivateMessageListModel PrepareInboxModel(int page, string tab);

        PrivateMessageListModel PrepareSentModel(int page, string tab);

        SendPrivateMessageModel PrepareSendPrivateMessageModel(Customer customerTo,
            PrivateMessage replyToPM);

        PrivateMessageModel PreparePrivateMessageModel(PrivateMessage pm);
    }
}
