using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Marketing;
using System;

namespace Nop.Services.Suppliers
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface ISupplierVoucherCouponService
    {
        void InsertEntity(SupplierVoucherCoupon entity);

        void DeleteEntity(SupplierVoucherCoupon entity, bool delete = false);

        void DeleteEntities(IList<SupplierVoucherCoupon> entities, bool deleted = false);

        void UpdateEntity(SupplierVoucherCoupon entity);

        void UpdateEntities(IList<SupplierVoucherCoupon> entities);

        SupplierVoucherCoupon GetEntityById(int id);

        List<SupplierVoucherCoupon> GetEntitiesByIds(int[] entityIds);

        List<SupplierVoucherCoupon> GetEntitiesBySupplier(int supplierId, int supplierShopId = 0);

        IPagedList<SupplierVoucherCoupon> GetEntities(
            string name = "",
            int supplierId = 0,
            int? supplierShopId = 0,
            AssetType? assetType = null,
            bool? getForFree = null,
            bool? published = null,
            bool? locked = null,
            bool? newUserGift = null,
            bool? offlineConsume = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

    }
}