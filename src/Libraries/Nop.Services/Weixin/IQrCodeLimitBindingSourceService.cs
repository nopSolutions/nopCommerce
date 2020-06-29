using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// IQrCodeLimitBindingSourceService Service interface
    /// </summary>
    public partial interface IQrCodeLimitBindingSourceService
    {
        #region QrCodeLimitBindingSource

        void InsertEntity(QrCodeLimitBindingSource entity);

        void DeleteEntity(QrCodeLimitBindingSource entity);

        void DeleteEntities(IList<QrCodeLimitBindingSource> entities);

        void UpdateEntity(QrCodeLimitBindingSource entity);

        void UpdateEntities(IList<QrCodeLimitBindingSource> entities);

        QrCodeLimitBindingSource GetEntityById(int id);

        QrCodeLimitBindingSource GetEntityByQrcodeLimitId(int qrCodeLimitId);

        List<QrCodeLimitBindingSource> GetEntitiesByIds(int[] wEntityIds);

        IPagedList<QrCodeLimitBindingSource> GetEntities(int qrCodeLimitId = 0, int supplierId = 0, int supplierShopId = 0, int productId = 0, bool? published = null, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion
    }
}