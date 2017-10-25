namespace Nop.Core.Domain.Localization
{
    /// <summary>
    /// 定义一个本地化字符串资源类
    /// Represents a locale string resource
    /// </summary>
    public partial class LocaleStringResource : BaseEntity
    {
        /// <summary>
        /// 语言id
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// 资源名称
        /// Gets or sets the resource name
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源值
        /// Gets or sets the resource value
        /// </summary>
        public string ResourceValue { get; set; }
        
        /// <summary>
        /// 所属语言
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }

}
