using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 资产收入表
    /// </summary>
    public partial class UserAssetIncomeHistory : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 使用说明
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 供应商SupplierID
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 供应商店铺SupplierShopID
        /// </summary>
        public int SupplierShopId { get; set; }
        /// <summary>
        /// 【SupplierVoucherCoupon.Id】ID
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreatedUserId { get; set; }
        /// <summary>
        /// 资产所属人ID，拥有人ID，没有赠送情况下=创建人ID
        /// </summary>
        public int OwnerUserId { get; set; }
        /// <summary>
        /// 赠送人用户Id，卡券类可赠送的（可用于统计卡券发放人绩效）
        /// </summary>
        public int GiverUserId { get; set; }
        /// <summary>
        /// 剩余转赠次数，避免来回不断赠送
        /// </summary>
        public int TransferTimes { get; set; }
        //获得该礼品对应的订单Id，系统赠送的没有订单号
        public int PurchasedWithOrderItemId { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 已使用值（针对没过期的积分，虚拟币，优先拆分使用快过期的；抵用券，折扣券类按整张使用，不拆分），实物卡，虚拟卡可拆分使用。
        /// </summary>
        public decimal UsedValue { get; set; }
        /// <summary>
        /// 满多少利润可使用。备用（只作前台是否可用筛选使用）
        /// </summary>
        public decimal UpProfitCanUsed { get; set; }
        /// <summary>
        /// 满多少件数量可使用。
        /// </summary>
        public decimal UpCountCanUsed { get; set; }
        /// <summary>
        /// 满多少可使用
        /// </summary>
        public decimal UpAmountCanUsed { get; set; }
        /// <summary>
        /// 优惠码（仅优惠券类型有值），实物卡，虚拟卡为卡号
        /// </summary>
        public string GiftCardCouponCode { get; set; }
        /// <summary>
        /// 【WAssetType】资产类型EnumID
        /// </summary>
        public byte AssetTypeId { get; set; }
        /// <summary>
        /// 资产类型EnumID
        /// </summary>
        public AssetType AssetType
        {
            get => (AssetType)AssetTypeId;
            set => AssetTypeId = (byte)value;
        }
        /// <summary>
        /// 【AssetConsumType】积分/账户收入方式EnumID
        /// </summary>
        public byte AssetConsumTypeId { get; set; }
        /// <summary>
        /// 积分/账户收入方式EnumID
        /// </summary>
        public AssetConsumType AssetConsumType
        {
            get => (AssetConsumType)AssetConsumTypeId;
            set => AssetConsumTypeId = (byte)value;
        }
        /// <summary>
        /// 【PromotionUseScopeType】使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品
        /// </summary>
        public byte PromotionUseScopeTypeId { get; set; }
        /// <summary>
        /// 使用范围类型
        /// </summary>
        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }
        /// <summary>
        /// 是否新用户才能使用
        /// </summary>
        public bool NewUserGift { get; set; }
        /// <summary>
        /// 必须审核通过才能使用。是否审核通过（对于商家促销赠送或合伙人营销赠送的，须要审核过后才能使用，审核过程主要是针对赠送人收起结算价格或广告费）
        /// </summary>
        public bool Approved { get; set; }
        /// <summary>
        /// 是否仅线下消费
        /// </summary>
        public bool OfflineConsume { get; set; }
        /// <summary>
        /// 卡券是否激活，激活卡券不能转赠（个人激活，部分卡激活后重新计算使用有效期）
        /// </summary>
        public bool IsGiftCardActivated { get; set; }
        /// <summary>
        /// 是否无效（购买订单退货或取消时扣除）
        /// </summary>
        public bool IsInvalid { get; set; }
        /// <summary>
        /// 是否完成（购买产品后，不一定完成，等待确认收货后，超过退货日期才算完成）
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 可以使用的开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 可以使用的结束时间/可使用最晚过期时间（即过期时间）
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
