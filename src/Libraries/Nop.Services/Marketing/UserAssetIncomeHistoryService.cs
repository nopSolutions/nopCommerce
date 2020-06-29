using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Weixin;
using Nop.Core.Html;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Nop.Services.Marketing
{
    /// <summary>
    /// WConfig Service
    /// </summary>
    public partial class UserAssetIncomeHistoryService : IUserAssetIncomeHistoryService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<UserAssetIncomeHistory> _userAssetIncomeHistoryRepository;
        private readonly IRepository<SupplierVoucherCoupon> _supplierVoucherCouponRepository;

        #endregion

        #region Ctor

        public UserAssetIncomeHistoryService(IEventPublisher eventPublisher,
            IRepository<UserAssetIncomeHistory> userAssetIncomeHistoryRepository,
            IRepository<SupplierVoucherCoupon> supplierVoucherCouponRepository
            )
        {
            _eventPublisher = eventPublisher;
            _userAssetIncomeHistoryRepository = userAssetIncomeHistoryRepository;
            _supplierVoucherCouponRepository = supplierVoucherCouponRepository;
        }

        #endregion

        #region Methods

        public virtual void InsertEntity(UserAssetIncomeHistory entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _userAssetIncomeHistoryRepository.Insert(entity);

            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public virtual void InsertEntityBysupplierVoucherCouponParams(
            SupplierVoucherCoupon supplierVoucherCoupon, 
            int ownerUserId, 
            int orderItemId=0,
            AssetConsumType? assetConsumType = null,
            WSceneType? sceneType = null)
        {
            if (supplierVoucherCoupon == null || ownerUserId == 0)
                return;

            var incomeHistory = new Nop.Core.Domain.Marketing.UserAssetIncomeHistory
            {
                Name = supplierVoucherCoupon.Name,
                Description = supplierVoucherCoupon.Description,
                Instructions = supplierVoucherCoupon.Instructions,
                Remark = "",
                SupplierId = supplierVoucherCoupon.SupplierId,
                SupplierShopId = supplierVoucherCoupon.SupplierShopId ?? 0,
                SupplierVoucherCouponId = supplierVoucherCoupon.Id,
                CreatedUserId = ownerUserId,
                OwnerUserId = ownerUserId,
                GiverUserId = 0,
                TransferTimes = supplierVoucherCoupon.TransferTimes,
                PurchasedWithOrderItemId = orderItemId,
                Amount = supplierVoucherCoupon.Amount,
                UsedValue = 0,
                UpProfitCanUsed = supplierVoucherCoupon.UpProfitCanUsed,
                UpCountCanUsed = supplierVoucherCoupon.UpCountCanUsed,
                UpAmountCanUsed = supplierVoucherCoupon.UpAmountCanUsed,
                GiftCardCouponCode = "", //卡券优惠码（采用系统生成）
                AssetTypeId = supplierVoucherCoupon.AssetTypeId,
                //AssetConsumType=AssetConsumType.Purchase  //要使用二维码场景类型和是否有订单号判断，并赋值
                PromotionUseScopeType = supplierVoucherCoupon.PromotionUseScopeType,
                NewUserGift = supplierVoucherCoupon.NewUserGift,
                Approved= supplierVoucherCoupon.AutoApproved, //要使用二维码场景类型判断，并赋值
                OfflineConsume = supplierVoucherCoupon.OfflineConsume,
                IsGiftCardActivated = supplierVoucherCoupon.AutoActive ? true : false,
                IsInvalid = true,
                Completed = false,
                Deleted = false,
                //StartDateTimeUtc= //需要判断
                EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime,
                CreatedOnUtc = DateTime.UtcNow
            };
            
            //设置AssetConsumType值
            if (orderItemId > 0)
            {
                incomeHistory.AssetConsumType = AssetConsumType.Purchase;
            }
            else if (assetConsumType != null)
            {
                incomeHistory.AssetConsumType = assetConsumType ?? AssetConsumType.SupplierPromotion;
            }
            else if (sceneType != null)
            {
                switch (sceneType)
                {
                    case WSceneType.Adver:
                    case WSceneType.Product:
                    case WSceneType.Supplier:
                        {
                            incomeHistory.AssetConsumType = AssetConsumType.SupplierPromotion;
                            break;
                        }
                    case WSceneType.GiftCard:
                        {
                            incomeHistory.AssetConsumType = AssetConsumType.PersonalPromotion;
                            break;
                        }
                    default:
                        {
                            incomeHistory.AssetConsumType = AssetConsumType.SupplierPromotion;
                            break;
                        }
                }
            }
            else
            {
                incomeHistory.AssetConsumType = AssetConsumType.SupplierPromotion;
            }

            //设置Approved值
            if (incomeHistory.AssetConsumType == AssetConsumType.PersonalPromotion && !supplierVoucherCoupon.AutoApproved)
                incomeHistory.Approved = false;
            else
                incomeHistory.Approved = true;

            //设置可用时间
            switch (supplierVoucherCoupon.ExpiredDateType)
            {
                
                case ExpiredDateType.MonthsFromNow:
                    {
                        if (supplierVoucherCoupon.AutoActive)
                        {
                            incomeHistory.StartDateTimeUtc = DateTime.UtcNow;
                            var calcDate = DateTime.UtcNow.AddMonths(supplierVoucherCoupon.ExpiredDays);
                            if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                incomeHistory.EndDateTimeUtc = calcDate;
                            else
                                incomeHistory.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                        }
                        break;
                    }
                case ExpiredDateType.DaysFromNow:
                    {
                        if (supplierVoucherCoupon.AutoActive)
                        {
                            incomeHistory.StartDateTimeUtc = DateTime.UtcNow;
                            var calcDate = DateTime.UtcNow.AddDays(supplierVoucherCoupon.ExpiredDays);
                            if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                incomeHistory.EndDateTimeUtc = calcDate;
                            else
                                incomeHistory.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                        }
                        break;
                    }
                case ExpiredDateType.HoursFromNow:
                    {
                        if (supplierVoucherCoupon.AutoActive)
                        {
                            incomeHistory.StartDateTimeUtc = DateTime.UtcNow;
                            var calcDate = DateTime.UtcNow.AddHours(supplierVoucherCoupon.ExpiredDays);
                            if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                incomeHistory.EndDateTimeUtc = calcDate;
                            else
                                incomeHistory.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                        }
                        break;
                    }
                case ExpiredDateType.FixDateTime:
                default:
                    {
                        incomeHistory.IsGiftCardActivated = true;
                        incomeHistory.StartDateTimeUtc = supplierVoucherCoupon.StartUseDateTime;
                        incomeHistory.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                        break;
                    }
            }

            //插入值
            InsertEntity(incomeHistory);
        }

        public virtual void DeleteEntity(UserAssetIncomeHistory entity, bool delete = false)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (delete)
            {
                _userAssetIncomeHistoryRepository.Delete(entity);
            }
            else
            {
                entity.Deleted = true;
                UpdateEntity(entity);
            }

            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        public virtual void DeleteEntities(IList<UserAssetIncomeHistory> entities, bool deleted = false)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (deleted)
            {
                _userAssetIncomeHistoryRepository.Delete(entities);
            }
            else
            {
                foreach (var entity in entities)
                {
                    entity.Deleted = true;
                }
                //delete wUser
                UpdateEntities(entities);
            }

            foreach (var entity in entities)
            {
                //event notification
                _eventPublisher.EntityDeleted(entity);
            }
        }

        public virtual void UpdateEntity(UserAssetIncomeHistory entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _userAssetIncomeHistoryRepository.Update(entity);

            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        public virtual void UpdateEntities(IList<UserAssetIncomeHistory> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            //update
            _userAssetIncomeHistoryRepository.Update(entities);

            //event notification
            foreach (var entity in entities)
            {
                _eventPublisher.EntityUpdated(entity);
            }
        }

        public virtual void ActiveEntity(int userAssetIncomeHistoryId)
        {
            var entity = GetEntityById(userAssetIncomeHistoryId);
            if (entity == null || entity.IsGiftCardActivated)
                return;

            //TOTO:激活卡券
            entity.IsGiftCardActivated = true;

            var supplierVoucherCoupon = _supplierVoucherCouponRepository.GetById(entity.SupplierVoucherCouponId);
            if (supplierVoucherCoupon != null)
            {
                //设置可用时间
                switch (supplierVoucherCoupon.ExpiredDateType)
                {
                    case ExpiredDateType.MonthsFromNow:
                        {
                            if (supplierVoucherCoupon.AutoActive)
                            {
                                entity.StartDateTimeUtc = DateTime.UtcNow;
                                var calcDate = DateTime.UtcNow.AddMonths(supplierVoucherCoupon.ExpiredDays);
                                if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                    entity.EndDateTimeUtc = calcDate;
                                else
                                    entity.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                            }
                            break;
                        }
                    case ExpiredDateType.DaysFromNow:
                        {
                            if (supplierVoucherCoupon.AutoActive)
                            {
                                entity.StartDateTimeUtc = DateTime.UtcNow;
                                var calcDate = DateTime.UtcNow.AddDays(supplierVoucherCoupon.ExpiredDays);
                                if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                    entity.EndDateTimeUtc = calcDate;
                                else
                                    entity.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                            }
                            break;
                        }
                    case ExpiredDateType.HoursFromNow:
                        {
                            if (supplierVoucherCoupon.AutoActive)
                            {
                                entity.StartDateTimeUtc = DateTime.UtcNow;
                                var calcDate = DateTime.UtcNow.AddHours(supplierVoucherCoupon.ExpiredDays);
                                if (calcDate < supplierVoucherCoupon.EndUseDateTime)
                                    entity.EndDateTimeUtc = calcDate;
                                else
                                    entity.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                            }
                            break;
                        }
                    case ExpiredDateType.FixDateTime:
                    default:
                        {
                            entity.IsGiftCardActivated = true;
                            entity.StartDateTimeUtc = supplierVoucherCoupon.StartUseDateTime;
                            entity.EndDateTimeUtc = supplierVoucherCoupon.EndUseDateTime;
                            break;
                        }
                }
            }

            //更新
            UpdateEntity(entity);
        }

        public virtual UserAssetIncomeHistory GetEntityById(int id)
        {
            if (id == 0)
                return null;

            return _userAssetIncomeHistoryRepository.ToCachedGetById(id);
        }

        public virtual List<UserAssetIncomeHistory> GetEntitiesByUserId(int wuserId)
        {
            if (wuserId == 0)
                return new List<UserAssetIncomeHistory>();

            var query = from t in _userAssetIncomeHistoryRepository.Table
                        where t.OwnerUserId == wuserId &&
                        !t.Deleted
                        select t;

            return query.ToList();
        }

        public virtual List<UserAssetIncomeHistory> GetEntitiesBySupplierId(
            int wuserId,
            int supplierId,
            int? supplierShopId = null,
            bool? onlyUsable = null)
        {
            if (wuserId == 0 || supplierId == 0)
                return new List<UserAssetIncomeHistory>();

            var query = _userAssetIncomeHistoryRepository.Table;
            query = query.Where(q => q.OwnerUserId == wuserId);
            query = query.Where(q => q.SupplierId == supplierId);
            if (supplierShopId.HasValue && supplierShopId > 0)
                query = query.Where(q => q.SupplierShopId == supplierShopId);
            if (onlyUsable.HasValue)
            {
                if (onlyUsable == true)
                {
                    query = query.Where(q => q.Amount > q.UsedValue); //卡券值没有使用完
                    query = query.Where(q => !q.IsInvalid);  //是否无效
                    query = query.Where(q => !q.Completed);  //是否完成交易
                    query = query.Where(q => q.EndDateTimeUtc >= DateTime.UtcNow);  //是否过期
                }
                else
                {
                    query = query.Where(q => (q.Amount <= q.UsedValue) || q.IsInvalid || q.Completed || q.EndDateTimeUtc < DateTime.UtcNow);
                }
            }

            query = query.Where(q => !q.Deleted);

            return query.ToList();
        }

        public virtual List<UserAssetIncomeHistory> GetEntitiesBySupplierVoucherCouponId(
            int wuserId, 
            int supplierVoucherCouponId, 
            bool? onlyUsable = null)
        {
            if (wuserId == 0 || supplierVoucherCouponId == 0)
                return new List<UserAssetIncomeHistory>();

            var query = _userAssetIncomeHistoryRepository.Table;
            query = query.Where(q => q.OwnerUserId == wuserId);
            query = query.Where(q => q.SupplierVoucherCouponId == supplierVoucherCouponId);
            if (onlyUsable.HasValue)
            {
                if (onlyUsable == true)
                {
                    query = query.Where(q => q.Amount > q.UsedValue); //卡券值没有使用完
                    query = query.Where(q => !q.IsInvalid);  //是否无效
                    query = query.Where(q => !q.Completed);  //是否完成交易
                    query = query.Where(q => q.EndDateTimeUtc >= DateTime.UtcNow);  //是否过期
                }
                else
                {
                    query = query.Where(q => (q.Amount <= q.UsedValue) || q.IsInvalid || q.Completed || q.EndDateTimeUtc < DateTime.UtcNow);
                }
            }

            query = query.Where(q => !q.Deleted);

            return query.ToList();
        }

        public virtual IPagedList<UserAssetIncomeHistory> GetEntities(
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
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userAssetIncomeHistoryRepository.Table;
            query = query.Where(q => q.OwnerUserId == ownerUserId);

            if (supplierId > 0)
                query = query.Where(q => q.SupplierId == supplierId);
            if (supplierShopId > 0)
                query = query.Where(q => q.SupplierShopId == supplierShopId);
            if (supplierVoucherCouponId > 0)
                query = query.Where(q => q.SupplierVoucherCouponId == supplierVoucherCouponId);
            if (!string.IsNullOrEmpty(giftCardCouponCode))
                query = query.Where(q => q.GiftCardCouponCode == giftCardCouponCode);
            if (assetType.HasValue)
                query = query.Where(q => q.AssetType == assetType);
            if (assetConsumType.HasValue)
                query = query.Where(q => q.AssetConsumType == assetConsumType);

            if (completed.HasValue)
                query = query.Where(q => q.Completed == completed);
            if (approved.HasValue)
                query = query.Where(q => q.Approved == approved);
            if (isInvalid.HasValue)
                query = query.Where(q => q.IsInvalid == isInvalid);
            if (deleted.HasValue)
                query = query.Where(q => q.Deleted == deleted);

            return new PagedList<UserAssetIncomeHistory>(query, pageIndex, pageSize);
        }


        #endregion
    }
}