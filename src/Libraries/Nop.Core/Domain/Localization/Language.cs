using System.Collections.Generic;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Localization
{
    /// <summary>
    /// 定义一个语言实体
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity, IStoreMappingSupported
    {
        private ICollection<LocaleStringResource> _localeStringResources;

        /// <summary>
        /// 名称
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 语言文化
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// 唯一SEO代码
        /// Gets or sets the unique SEO code
        /// </summary>
        public string UniqueSeoCode { get; set; }
        
        /// <summary>
        /// 国旗图像文件名称
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// 是否启用文字从右向左支持
        /// Gets or sets a value indicating whether the language supports "Right-to-left"
        /// </summary>
        public bool Rtl { get; set; }

        /// <summary>
        /// 商城限制
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }

        /// <summary>
        /// 默认货币
        /// Gets or sets the identifier of the default currency for this language; 0 is set when we use the default currency display order
        /// </summary>
        public int DefaultCurrencyId { get; set; }

        /// <summary>
        /// 是否发布
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// 显示顺序
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// 字符串资源
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources
        {
            get { return _localeStringResources ?? (_localeStringResources = new List<LocaleStringResource>()); }
            protected set { _localeStringResources = value; }
        }
    }
}
