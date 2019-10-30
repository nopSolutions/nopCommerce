using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum group
    /// </summary>
    [Table(NopMappingDefaults.ForumsGroupTable)]
    public partial class ForumGroup : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Column]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [Column]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [Column]
        public DateTime UpdatedOnUtc { get; set; }
    }
}
