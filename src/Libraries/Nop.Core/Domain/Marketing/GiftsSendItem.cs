using System;
using Humanizer;
using Nop.Core.Domain.Discounts;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 礼品赠送/领取设置表
    /// </summary>
    public partial class GiftsSendItem : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 绑定的产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 赠送用户角色限制
        /// </summary>
        public int? CustomerRoleId { get; set; }
        /// <summary>
        /// Store限制
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 最小购买数量，没有达到不参与赠送，最小值1
        /// </summary>
        public int MinQuantity { get; set; }
        /// <summary>
        /// 最大购买数量，超出部分不参与赠送，0为不限制上限
        /// </summary>
        public int MaxQuantity { get; set; }
        /// <summary>
        /// 是否启用使用范围限制
        /// </summary>
        public bool PromotionUseScopeEnable { get; set; }
        /// <summary>
        /// 【PromotionUseScopeType】使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品
        /// </summary>
        public byte PromotionUseScopeTypeId { get; set; }
        /// <summary>
        /// 使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品
        /// </summary>
        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }
        /// <summary>
        /// 指定的IDs值，启用，未指定时，默认为仅当前产品的对应的店铺，频道，类别属性中
        /// </summary>
        public string PromotionScopeValues { get; set; }
        /// <summary>
        /// 是否启用利润空间限制
        /// </summary>
        public bool ProfitUseScopeEnable { get; set; }
        /// <summary>
        /// 利润最小值
        /// </summary>
        public decimal ProfitScopeValue { get; set; }
        /// <summary>
        /// 总结算价限制
        /// </summary>
        public bool TotalAmountUseScopeEnable { get; set; }
        /// <summary>
        /// 总结算价最小金额
        /// </summary>
        public decimal TotalAmountScopeValue { get; set; }
        /// <summary>
        /// 赠送物品的数值，必须大于0，检查值是否大于销售价格-成本价格（报错）
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 步进计数值，（买1送1，步进为1，买5送1，步进为5）
        /// </summary>
        public int StepCount { get; set; }
        /// <summary>
        /// 允许转赠次数
        /// </summary>
        public int TransferTimes { get; set; }
        /// <summary>
        /// 数值使用百分比
        /// </summary>
        public bool UsePercentage { get; set; }
        /// <summary>
        /// 比例 检查值是否大于销售价格-成本价格（报错）
        /// </summary>
        public decimal Percentage { get; set; }
        /// <summary>
        /// 如果采用百分比，计算结果不能大于该值检查值是否大于销售价格-成本价格（报错）
        /// </summary>
        public decimal MaxPercentageAmount { get; set; }
        /// <summary>
        /// Enum表【WAssetType】绑定类型：虚拟现金/积分/虚拟币/实物卡/虚拟卡/抵用券等
        /// </summary>
        public byte AssetTypeId { get; set; }
        /// <summary>
        /// 绑定类型：虚拟现金/积分/虚拟币/实物卡/虚拟卡/抵用券等
        /// </summary>
        public AssetType AssetType
        {
            get => (AssetType)AssetTypeId;
            set => AssetTypeId = (byte)value;
        }
        /// <summary>
        /// 【ExpiredDateType】使用期限类型； 1=自购买之日起x天，最大365天，2=固定到期日期，最大距离当前时间1年
        /// </summary>
        public byte ExpiredDateTypeId { get; set; }
        /// <summary>
        /// 过期期限类型； 1=自购买之日起x天，最大365天，2=固定到期日期，最大距离当前时间1年
        /// </summary>
        public ExpiredDateType ExpiredDateType
        {
            get => (ExpiredDateType)ExpiredDateTypeId;
            set => ExpiredDateTypeId = (byte)value;
        }
        /// <summary>
        /// 【GiftCard.Id】卡券，抵用券ID
        /// </summary>
        public int? GiftCardId { get; set; }
        /// <summary>
        /// 使用限制：0=不限制，15=绑定的产品限制总次数，25=每个用户限制总次数
        /// </summary>
        public byte DiscountLimitationTypeId { get; set; }
        public DiscountLimitationType DiscountLimitationType
        {
            get => (DiscountLimitationType)DiscountLimitationTypeId;
            set => DiscountLimitationTypeId = (byte)value;
        }
        /// <summary>
        /// 限制次数
        /// </summary>
        public int LimitTimes { get; set; }
        /// <summary>
        /// 1=自购买之日起x天
        /// </summary>
        public int ExpiredDays { get; set; }
        /// <summary>
        /// 2=固定到期日期
        /// </summary>
        public DateTime? ExpiredDateTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 是否仅线下才能使用
        /// </summary>
        public bool OfflineConsume { get; set; }
        /// <summary>
        /// 是否新用户才赠送
        /// </summary>
        public bool NewUserGift { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除（不真正删除）
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
