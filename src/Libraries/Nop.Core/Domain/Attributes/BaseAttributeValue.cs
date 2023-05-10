using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Attributes
{
    /// <summary>
    /// Represents the base class for attribute values
    /// </summary>
    public abstract partial class BaseAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the attribute identifier
        /// </summary>
        public int AttributeId { get; set; }
    }
}
