using System;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsCategory : BaseEntity
    {
        public int StoreId { get; set; }

        public string Title{ get; set; }

        public string SubTitle { get; set; }

        public int ParentId { get; set; }

        public string ClassList { get; set; }

        public string ClassLayer { get; set; }

        public int DisplayOrder { get; set; }

        public string ImageUrl { get; set; }

        public string ShortContent { get; set; }

        public string Content { get; set; }

        public string MetaTitle { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string ViewIndexTemplateName { get; set; }

        public string ViewListTemplateName { get; set; }

        public string ViewDetailTemplateName { get; set; }

        public bool Published { get; set; }
    }
}