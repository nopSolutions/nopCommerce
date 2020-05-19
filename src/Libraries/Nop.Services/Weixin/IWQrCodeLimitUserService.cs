using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WQrCodeLimitUser Service interface
    /// </summary>
    public partial interface IWQrCodeLimitUserService
    {
        #region WQrCodeLimitUserMapping

        void InsertEntity(WQrCodeLimitUserMapping entity);

        void DeleteEntity(WQrCodeLimitUserMapping entity, bool delete = false);

        void DeleteEntities(IList<WQrCodeLimitUserMapping> entities, bool deleted = false);

        void UpdateEntity(WQrCodeLimitUserMapping entity);

        void UpdateEntities(IList<WQrCodeLimitUserMapping> entities);

        WQrCodeLimitUserMapping GetEntityById(int id);

        WQrCodeLimitUserMapping GetEntityByQrcodeLimitId(int qrCodeLimitId);

        List<WQrCodeLimitUserMapping> GetEntitiesByIds(int[] wEntityIds);

        IPagedList<WQrCodeLimitUserMapping> GetEntities(string openId = "", int qrCodeLimitId = 0, int? expireTime = null, bool? published = null, bool showDeleted = false, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion
    }
}