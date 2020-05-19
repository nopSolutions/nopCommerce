using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WUserTag service interface
    /// </summary>
    public partial interface IWUserTagService
    {
        void InsertWUserTag(WUserTag userTag);

        void DeleteWUserTag(WUserTag userTag, bool delete = false);

        void DeleteWUserTags(IList<WUserTag> userTags, bool deleted = false);

        void UpdateWUserTag(WUserTag userTag);

        void UpdateWUserTags(IList<WUserTag> userTags);

        WUserTag GetWUserTagById(int id);

        WUserTag GetWUserTagByOfficialId(int officialId, int? configId = null);

        List<WUserTag> GetWUserTagsByOfficialIds(int[] officialIds, int? configId = null);

        IPagedList<WUserTag> GetWUserTags(int? configId = null, int pageIndex = 0, int pageSize = int.MaxValue, bool showDeleted = false);
    }
}