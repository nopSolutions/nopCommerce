using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品活动主题绑定表
    /// </summary>
    public partial class ProductActivitiesThemeMapping : BaseEntity
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 【ActivitiesTheme.Id】
        /// </summary>
        public int ActivitiesThemeId { get; set; }
        /// <summary>
        /// 是否发布，发布前需要审核
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
