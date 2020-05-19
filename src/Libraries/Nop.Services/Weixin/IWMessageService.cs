using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WMessage Service interface
    /// </summary>
    public partial interface IWMessageService
    {
        void InsertWMessage(WMessage wMessage);

        void DeleteWMessage(WMessage wMessage, bool delete = false);

        void DeleteWMessages(IList<WMessage> wMessages, bool deleted = false);

        void UpdateWMessage(WMessage wMessage);

        void UpdateWMessages(IList<WMessage> wMessages);

        WMessage GetWMessageById(int id);

        List<WMessage> GetWMessagesByIds(int[] wMessageIds);

        IPagedList<WMessage> GetWMessages(string title = "", bool? published = null, bool? deleted = null, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}