using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 合伙人在AllowCreateVoucher或AllowCreateCoupon允许情况下的个人营销行为
    /// 从表中选择可以为客户自主生成的折扣券或抵用券
    /// </summary>
    public partial class UserCreateProductCoupon : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 产品ID，绑定产品的，该字段须有值>0
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 供应商Id，绑定供应商的，该字段需要有值
        /// </summary>
        public int? SupplierId { get; set; }
        /// <summary>
        /// 供应商店铺Id，绑定供应商店铺的，该字段需要有值
        /// </summary>
        public int? SupplierShopId { get; set; }
        /// <summary>
        /// 永久不变，领券地址（该地址主要用于券类的详细说明和临时生成二维码，关注后，领取对应的优惠券）
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 必须大于0
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Enum表【AssetType】绑定类型：虚拟现金/积分/虚拟币/实物卡/虚拟卡/抵用券/折扣券等
        /// </summary>
        public byte AssetTypeId { get; set; }
        /// <summary>
        /// 绑定类型：虚拟现金/积分/虚拟币/实物卡/虚拟卡/抵用券/折扣券等
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
        /// 使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品
        /// </summary>
        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }
        /// <summary>
        /// 【ExpiredDateType】使用期限类型； 1=自购买之日起x天，最大365天，2=固定到期日期，最大距离当前时间1年
        /// </summary>
        public byte ExpiredDateTypeId { get; set; }
        /// <summary>
        /// 使用期限类型； 1=自购买之日起x天，最大365天，2=固定到期日期，最大距离当前时间1年
        /// </summary>
        public ExpiredDateType ExpiredDateType
        {
            get => (ExpiredDateType)ExpiredDateTypeId;
            set => ExpiredDateTypeId = (byte)value;
        }
        /// <summary>
        /// 1=自购买之日起x天
        /// </summary>
        public int ExpiredDays { get; set; }
        /// <summary>
        /// 可以使用的开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 可以使用的结束时间（即过期时间）
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }
        /// <summary>
        /// 最大发放券总数量
        /// </summary>
        public int MaxGiveCount { get; set; }
        /// <summary>
        /// 是否是供应商提供的卡券（否则个人只能生成指定产品的卡券）
        /// </summary>
        public bool IsSupplier { get; set; }
        /// <summary>
        /// 发布（供应商可以自己控制是否发放状态）
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 锁定（由系统管理员设置）
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// 是否新用户才赠送
        /// </summary>
        public bool NewUserGift { get; set; }
        /// <summary>
        /// 是否仅线下消费
        /// </summary>
        public bool OfflineConsume { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime CreatedOnUtc { get; set; }

    }
}
