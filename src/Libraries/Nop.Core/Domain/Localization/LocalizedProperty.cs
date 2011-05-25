namespace Nop.Core.Domain.Localization
{
    /// <summary>
    /// Represents a localized property
    /// </summary>
    public partial class LocalizedProperty : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the locale key group
        /// </summary>
        public virtual string LocaleKeyGroup { get; set; }

        /// <summary>
        /// Gets or sets the locale key
        /// </summary>
        public virtual string LocaleKey { get; set; }

        /// <summary>
        /// Gets or sets the locale value
        /// </summary>
        public virtual string LocaleValue { get; set; }
        
        /// <summary>
        /// Gets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }
}
