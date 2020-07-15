using System;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsTags : BaseEntity
    {
        public string Name { get; set; }
        public bool IsRed { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatOnUtc { get; set; }
    }
}