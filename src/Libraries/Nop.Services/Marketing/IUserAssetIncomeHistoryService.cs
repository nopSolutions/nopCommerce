using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Suppliers;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// Suppliers Service interface
    /// </summary>
    public partial interface IUserAssetIncomeHistoryService
    {
        void InsertEntity(UserAssetIncomeHistory entity);

        void InsertEntityBysupplierVoucherCouponParams(SupplierVoucherCoupon supplierVoucherCoupon, int ownerUserId, int orderItemId = 0, AssetConsumType? assetConsumType = null, WSceneType? sceneType = null);

        void DeleteEntity(UserAssetIncomeHistory entity, bool delete = false);

        void DeleteEntities(IList<UserAssetIncomeHistory> entities, bool deleted = false);

        void UpdateEntity(UserAssetIncomeHistory entity);

        void UpdateEntities(IList<UserAssetIncomeHistory> entities);
        /// <summary>
        /// 没有激活的卡券进行激活操作
        /// </summary>
        /// <param name="entity"></param>
        void ActiveEntity(int userAssetIncomeHistoryId);

        UserAssetIncomeHistory GetEntityById(int id);

        List<UserAssetIncomeHistory> GetEntitiesByUserId(int wuserId);

        List<UserAssetIncomeHistory> GetEntitiesBySupplierId(int wuserId, int supplierId, int? supplierShopId = null, bool? onlyUsable = null);

        List<UserAssetIncomeHistory> GetEntitiesBySupplierVoucherCouponId(int wuserId, int supplierVoucherCouponId, bool? onlyUsable = null);

        IPagedList<UserAssetIncomeHistory> GetEntities(
            int ownerUserId = 0,
            int supplierId = 0,
            int supplierShopId = 0,
            int supplierVoucherCouponId = 0,
            string giftCardCouponCode = "",
            AssetType? assetType = null,
            AssetConsumType? assetConsumType = null,
            bool? completed = null,
            bool? approved = null,
            bool? isInvalid = null,
            bool? deleted = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}