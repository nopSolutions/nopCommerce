using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// Represents an affiliate
    /// </summary>
    public partial class Affiliate : BaseEntity
    {
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }

        public virtual Address Address { get; set; }
    }
}
