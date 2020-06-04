using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品扩展标签表：如7天无理由退货等服务类型标签
    /// </summary>
    public partial class ProductExtendLabel : BaseEntity
    {
        /// <summary>
        /// 标签名称(对外展示名称)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签名称(后台展示名称)
        /// </summary>
        public string SysName { get; set; }

        /// <summary>
        /// 标签说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 限制站点
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 限制栏目
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
    }
}
