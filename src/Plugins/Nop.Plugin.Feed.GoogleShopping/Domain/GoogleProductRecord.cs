using Nop.Core;

namespace Nop.Plugin.Feed.GoogleShopping.Domain
{
    /// <summary>
    /// Represents a Google product record
    /// </summary>
    public partial class GoogleProductRecord : BaseEntity
    {
        public int ProductId { get; set; }
        public string Taxonomy { get; set; }

        /// <summary>
        /// A value indicating whether it's custom goods (not unique identifier exists)
        /// </summary>
        public bool CustomGoods { get; set; }
        public string Gender { get; set; }
        public string AgeGroup { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Material { get; set; }
        public string Pattern { get; set; }
        public string ItemGroupId { get; set; }
    }
}