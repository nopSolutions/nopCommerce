using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 营销广告途径/方式表：易拉宝，店铺贴广告等
    /// </summary>
    public partial class MarketingAdvertWay : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
