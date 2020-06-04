using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 商品营销标签分类
    /// </summary>
    public partial class ProductVisitorMapping : BaseEntity
    {
        /// <summary>
        /// 产品Id,主键
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 访客Id列表，以逗号分开（只保留最新30个）
        /// </summary>
        /// <returns></returns>
        public string VisitorIds { get; set; }

    }
}
