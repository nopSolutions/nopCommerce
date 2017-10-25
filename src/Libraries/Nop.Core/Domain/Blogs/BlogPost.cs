using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Blogs
{
    /// <summary>
    /// 博客发帖信息
    /// Represents a blog post
    /// </summary>
    public partial class BlogPost : BaseEntity, ISlugSupported, IStoreMappingSupported
    {
        /// <summary>
        /// 博文评论集合
        /// </summary>
        private ICollection<BlogComment> _blogComments;

        /// <summary>
        /// 语言编号
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// 标题
        /// Gets or sets the blog post title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 正文内容
        /// Gets or sets the blog post body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the blog post overview. If specified, then it's used on the blog page instead of the "Body"
        /// 博文摘要，预览
        /// </summary>
        public string BodyOverview { get; set; }

        /// <summary>
        /// 是否允许评论
        /// Gets or sets a value indicating whether the blog post comments are allowed 
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// 标签
        /// Gets or sets the blog tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 开始日期时间
        /// Gets or sets the blog post start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// 结束日期时间
        /// Gets or sets the blog post end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// 标签关键字
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// 标签描述
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// 标签标题
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// 是否用于商店
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public virtual bool LimitedToStores { get; set; }
 
        /// <summary>
        /// 创建时间
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the blog comments
        /// 博文评论集合
        /// </summary>
        public virtual ICollection<BlogComment> BlogComments
        {
            get { return _blogComments ?? (_blogComments = new List<BlogComment>()); }
            protected set { _blogComments = value; }
        }
        
        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }
}