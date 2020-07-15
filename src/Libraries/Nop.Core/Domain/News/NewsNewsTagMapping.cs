using System;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsNewsTagMapping : BaseEntity
    {
        public int NewsId { get; set; }
        public int NewsTagId { get; set; }
    }
}