using Nop.Core;

namespace Nop.Plugin.Feed.Froogle.Domain
{
    /// <summary>
    /// Represents a Google product record
    /// </summary>
    public partial class GoogleProductRecord : BaseEntity
    {
        public virtual int ProductVariantId { get; set; }
        public virtual string Taxonomy { get; set; }

        //TODO allow store owner to edit the following fields
        public virtual string Gender { get; set; }
        public virtual string AgeGroup { get; set; }
        public virtual string Color { get; set; }
        public virtual string Size { get; set; }
        public virtual string Material { get; set; }
        public virtual string Pattern { get; set; }
        public virtual string ItemGroupId { get; set; }
    }
}