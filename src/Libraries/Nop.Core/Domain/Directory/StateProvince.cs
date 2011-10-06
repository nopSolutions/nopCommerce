using System.Collections.Generic;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents a state/province
    /// </summary>
    public partial class StateProvince : BaseEntity
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public virtual int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public virtual string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }
    }

}
