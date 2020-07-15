using System;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsAlbums : BaseEntity
    {
        public int NewsId { get; set; }
        public int DisplayOrder { get; set; }

        public bool Published { get; set; }
        public string ThumbPath { get; set; }
        public string OriginalPath { get; set; }
        public string Remark { get; set; }
        public DateTime CreatOnUtc { get; set; }
    }
}