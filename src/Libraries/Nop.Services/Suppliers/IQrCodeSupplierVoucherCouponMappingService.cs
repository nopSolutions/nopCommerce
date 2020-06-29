using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IQrCodeSupplierVoucherCouponMappingService
    {
        void InsertEntity(QrCodeSupplierVoucherCouponMapping entity);

        void DeleteEntity(QrCodeSupplierVoucherCouponMapping entity);

        void DeleteEntities(IList<QrCodeSupplierVoucherCouponMapping> entities);

        void UpdateEntity(QrCodeSupplierVoucherCouponMapping entity);

        void UpdateEntities(IList<QrCodeSupplierVoucherCouponMapping> entities);

        QrCodeSupplierVoucherCouponMapping GetEntityById(int id);

        List<QrCodeSupplierVoucherCouponMapping> GetEntitiesByIds(int[] entityIds);

        List<QrCodeSupplierVoucherCouponMapping> GetEntitiesByQrCodeId(int qrCodeId, bool qrcodeLimitId = true, bool showAll = false);

        IPagedList<QrCodeSupplierVoucherCouponMapping> GetEntities(
            int qrCodeId = 0,
            int supplierVoucherCouponId = 0,
            bool? qrcodeLimit = null,
            bool? published = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

    }
}