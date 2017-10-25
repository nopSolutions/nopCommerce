
namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// 类别模版
    /// Represents a category template
    /// </summary>
    public partial class CategoryTemplate : BaseEntity
    {
        /// <summary>
        /// 名称
        /// Gets or sets the template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 视图路径
        /// Gets or sets the view path
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// 排序次序
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
