using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 广告宣传地点
    /// </summary>
    public partial class MarketingAdvertAddress : BaseEntity
    {
        /// <summary>
        /// 地点名称
        /// </summary>
        public string Address { get; set; } 
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
    }
}
