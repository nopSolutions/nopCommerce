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
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 【GiftsSendItem.Id】礼品赠送/领取设置表
        /// </summary>
        public int? GiftsSendItemId { get; set; }
        /// <summary>
        /// 资产所属人ID，领取用户ID
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 赠送人用户Id，卡券类可赠送的
        /// </summary>
        public int? GiverId { get; set; }
        /// <summary>
        /// 已转赠次数，避免来回不断赠送
        /// </summary>
        public int TransferTimes { get; set; }
        /// <summary>
        /// 获得该礼品对应的订单Id，系统赠送的没有订单号
        /// </summary>
        public int? PurchasedWithOrderItemId { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SupplierId { get; set; }
        /// <summary>
        /// 供应商店铺ID
        /// </summary>
        public int? SupplierShopId { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 已使用值（针对没过期的积分，虚拟币，优先拆分使用快过期的；抵用券，折扣券类按整张使用，不拆分），实物卡，虚拟卡可拆分使用。
        /// </summary>
        public decimal UsedValue { get; set; }
        /// <summary>
        /// 优惠码（仅券类型有值），实物卡，虚拟卡为卡号
        /// </summary>
        public string GiftCardCouponCode { get; set; }
        /// <summary>
        /// 【AssetType】资产类型EnumID
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
        /// 使用范围
        /// </summary>
        public byte PromotionUseScopeTypeId { get; set; }
        /// <summary>
        /// 使用范围
        /// </summary>
        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }
        /// <summary>
        /// 使用范围指定的IDs值
        /// </summary>
        public string PromotionScopeValues { get; set; }
        /// <summary>
        /// 资源是否审核通过，必须通过才能使用
        /// 主要用于对于商家促销赠送或合伙人营销赠送的，审核过程主要是针对赠送人收结算价格或广告费
        /// </summary>
        public bool Approved { get; set; }
        /// <summary>
        /// 是否仅线下消费
        /// </summary>
        public bool OfflineConsume { get; set; }
        /// <summary>
        /// 卡券是否激活
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
        /// 是否展示
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 可以使用的开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 可以使用的结束时间（即过期时间）
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
