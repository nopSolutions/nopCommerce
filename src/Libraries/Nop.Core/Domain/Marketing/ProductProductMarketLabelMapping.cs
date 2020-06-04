using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 商品营销标签映射表
    /// </summary>
    public partial class ProductProductMarketLabelMapping : BaseEntity
    {
        /// <summary>
        /// 营销标签ID
        /// </summary>
        public int ProductMarketLabelId { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
