using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum group
    /// </summary>
    public partial class ForumGroup : BaseEntity
    {
        private ICollection<Forum> _forums;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the collection of Forums
        /// </summary>
        public virtual ICollection<Forum> Forums
        {
            get => _forums ?? (_forums = new List<Forum>());
            protected set => _forums = value;
        }
    }
}
