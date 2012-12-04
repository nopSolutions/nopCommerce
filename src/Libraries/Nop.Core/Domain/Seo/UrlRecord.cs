namespace Nop.Core.Domain.Seo
{
    /// <summary>
    /// Represents an URL record
    /// </summary>
    public partial class UrlRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity name
        /// </summary>
        public virtual string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the slug
        /// </summary>
        public virtual string Slug { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the record is active
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int LanguageId { get; set; }
    }
}
