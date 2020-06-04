using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 推广佣金表：产品可分配促销价和佣金总金额设置
    /// </summary>
    public partial class PromotionCommission : BaseEntity
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 合伙人1结算价
        /// </summary>
        public decimal PartnerClearingPrice1 { get; set; }
        /// <summary>
        /// 合伙人2结算价
        /// </summary>
        public decimal PartnerClearingPrice2 { get; set; }
        /// <summary>
        /// 合伙人3结算价
        /// </summary>
        public decimal PartnerClearingPrice3 { get; set; }
        /// <summary>
        /// 可分配金额固定值（检查值是否大于销售价格-成本价格）
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 百分比（这里要计算最大可用金额：销售价格-成本价格）没有设置成本价格，设置百分比无效
        /// </summary>
        public decimal Percentage { get; set; }
        /// <summary>
        /// 可分配金额是否使用百分比
        /// </summary>
        public bool UsePercentage { get; set; }
        /// <summary>
        /// 新用户分配比例37%
        /// </summary>
        public decimal NewUserPercentage { get; set; }
        /// <summary>
        /// 普通客户分配比例27%，VIP0
        /// </summary>
        public decimal GeneralPercentage { get; set; }
        /// <summary>
        /// 销售价格27%+33%佣金=60%
        /// </summary>
        public decimal VIP1Percentage { get; set; }
        /// <summary>
        /// 销售价格27%+38%佣金=65%
        /// </summary>
        public decimal VIP2Percentage { get; set; }
        /// <summary>
        /// 年卡会员销售价格37%，佣金比例参考Vip1或Vip2
        /// </summary>
        public decimal AnnualCardPercentage { get; set; }
        /// <summary>
        /// 最小购买数量，没有达到不参与分配
        /// </summary>
        public int MinQuantity { get; set; }
        /// <summary>
        /// 最大购买数量，超出部分不参与分配
        /// </summary>
        public int MaxQuantity { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 是否发布（产品价格和成本价改动时，暂停发布，以防分配价格出错）
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
