using System;
using Nop.Core.Domain.Marketing;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 供应商创建的卡片或券类
    /// </summary>
    public partial class SupplierVoucherCoupon : BaseEntity
    {
        /// <summary>
        /// 系统名称（全名，用于识别相似卡券）
        /// </summary>
        public string SystemName { get; set; }  
        /// <summary>
        /// 名称（用于在前台和用户卡券列表显示）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 使用说明
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// 卡券图片：横图/大图
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 供应商Id(必须)，绑定供应商的，该字段需要有值（必须有值，最大限度是在同供应商之内使用）
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 供应商店铺Id，绑定供应商店铺的，该字段需要有值
        /// </summary>
        public int? SupplierShopId { get; set; }
        /// <summary>
        /// 【PromotionUseScopeType】使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品（这里只允许使用供应商内，店铺内，指定商品类）
        /// </summary>
        public byte PromotionUseScopeTypeId { get; set; }
        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }
        /// <summary>
        /// Enum表【AssetType】资产类型：虚拟现金/积分/虚拟币/实物卡/虚拟卡/抵用券/折扣券等（目前只能使用虚拟卡，抵用券，折扣券），其他值备用，积分和虚拟币通过购买产品直接到账户（设置有效期1年），实物卡单产品直接卖，不入后台资产账户信息，或通过抵用券到线下换取实物卡。
        /// </summary>
        public byte AssetTypeId { get; set; }
        public AssetType AssetType
        {
            get => (AssetType)AssetTypeId;
            set => AssetTypeId = (byte)value;
        }
        /// <summary>
        /// 最大卡片数量，卡片总数 。卡片总数=购买赠送+营销赠送
        /// </summary>
        public int MaxCardsNumber { get; set; }
        /// <summary>
        /// 最大营销赠送数量发放券总数量。0=不限制
        /// </summary>
        public int MaxGiveNumber { get; set; }

        /// <summary>
        /// 每人最大限制领取总数日期范围。X天以内限制领取总数。0=全部日期范围内。
        /// </summary>
        public int LimitReceiveNumberDays{ get; set; }
        /// <summary>
        /// 每人最大限制领取总数，包括已经消费或过期的（主要用于免费领取的卡券类，购买获得的卡片类看实际情况使用该值）
        /// </summary>
        public int LimitReceiveNumber { get; set; }
        /// <summary>
        /// 每人最大限制未使用该卡券的数量/最大可用总数（如果使用或过期后，可以重复领取，但要保证在最大领取总数之内）
        /// </summary>
        public int LimitUsableNumber { get; set; }
        /// <summary>
        /// 允许转赠次数：0=不允许转赠
        /// </summary>
        public int TransferTimes { get; set; }
        /// <summary>
        /// 折扣比例（卡类型为折扣券时启用）
        /// </summary>
        public decimal Percentage { get; set; }
        /// <summary>
        /// 最大抵扣价。（卡类型为折扣券时启用）
        /// </summary>
        public decimal MaxDiscountAmount { get; set; }
        /// <summary>
        /// 必须大于0
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 满多少金额可使用。
        /// </summary>
        public decimal UpAmountCanUsed { get; set; }
        /// <summary>
        /// 满多少利润可使用。备用（只作前台是否可用筛选使用）
        /// </summary>
        public decimal UpProfitCanUsed { get; set; }
        /// <summary>
        /// 满多少件数量可使用。
        /// </summary>
        public int UpCountCanUsed { get; set; }
        /// <summary>
        /// 【ExpiredDateType】使用期限类型；自购买或激活之日起x时/x天/x月/固定时间
        /// </summary>
        public byte ExpiredDateTypeId { get; set; }
        public ExpiredDateType ExpiredDateType
        {
            get => (ExpiredDateType)ExpiredDateTypeId;
            set => ExpiredDateTypeId = (byte)value;
        }
        /// <summary>
        /// 自购买之日起x天过期
        /// </summary>
        public int ExpiredDays { get; set; }
        /// <summary>
        /// 固定起始日期和到期日期
        /// </summary>
        public DateTime? StartUseDateTime { get; set; }
        /// <summary>
        /// 固定起始日期和到期日期/或最大到期时间
        /// </summary>
        public DateTime EndUseDateTime { get; set; }
        /// <summary>
        /// 针对非购买方式获取的卡券，是否自动审核通过（否则需要人工在用户收入表中审核，同用户收入表中的Approved取值有关）
        /// </summary>
        public bool AutoApproved { get; set; }
        /// <summary>
        /// 是否可免费领取/否则必须通过产品购买才能获取，购买价格在产品售价中设置
        /// </summary>
        public bool GetForFree { get; set; }
        /// <summary>
        /// 是否自动激活
        /// </summary>
        public bool AutoActive { get; set; }
        /// <summary>
        /// 是否允许合伙人营销推广自己赠送（供应商可以自己控制是否发放状态）
        /// </summary>
        public bool AllowPartnerGive { get; set; }
        /// <summary>
        /// 发布（供应商可以自己控制是否发放状态）
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 锁定（由系统管理员设置）
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 是否新用户才赠送/新用户使用
        /// </summary>
        public bool NewUserGift { get; set; }
        /// <summary>
        /// 是否仅线下消费
        /// </summary>
        public bool OfflineConsume { get; set; }
        /// <summary>
        /// 删除（不直接删除，直到资产收入和消费表过期3个月才能删除）
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreatUserId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
