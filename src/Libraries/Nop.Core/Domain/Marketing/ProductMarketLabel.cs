using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 商品营销标签分类
    /// </summary>
    public partial class ProductMarketLabel : BaseEntity
    {
        /// <summary>
        /// 标签名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
    }
}
