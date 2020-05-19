using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WConfig Service interface
    /// </summary>
    public partial interface IWConfigService
    {
        void InsertWConfig(WConfig wConfig);

        void DeleteWConfig(WConfig wConfig, bool delete = false);

        void DeleteWConfigs(IList<WConfig> wConfigs, bool deleted = false);

        void UpdateWConfig(WConfig wConfig);

        void UpdateWConfigs(IList<WConfig> wConfigs);

        WConfig GetWConfigById(int id);

        WConfig GetWUserByOriginalId(string originalId);

        WConfig GetWConfigByStoreId(int storeId);

        List<WConfig> GetWConfigsByIds(int[] wConfigIds);

        IPagedList<WConfig> GetUsers(bool showDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}