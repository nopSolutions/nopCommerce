using System;
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

        void DeleteEntity(WQrCodeLimitUserMapping entity);

        void DeleteEntities(IList<WQrCodeLimitUserMapping> entities);

        void UpdateEntity(WQrCodeLimitUserMapping entity);

        void UpdateEntities(IList<WQrCodeLimitUserMapping> entities);

        WQrCodeLimitUserMapping GetEntityById(int id);

        WQrCodeLimitUserMapping GetActiveEntityByQrCodeLimitIdOrUserId(int qrCodeLimitId, int userId);

        WQrCodeLimitUserMapping GetEntityByQrCodeLimitIdAndUserId(int qrCodeLimitId, int userId);

        List<WQrCodeLimitUserMapping> GetEntitiesByQrcodeLimitId(int qrCodeLimitId);

        List<WQrCodeLimitUserMapping> GetEntitiesByUserId(int userId);

        List<WQrCodeLimitUserMapping> GetEntitiesByIds(int[] wEntityIds);

        IPagedList<WQrCodeLimitUserMapping> GetEntities(int userId = 0, int qrCodeLimitId = 0, DateTime? expireTime = null, bool? published = null, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion
    }
}