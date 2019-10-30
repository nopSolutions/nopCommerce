using LinqToDB.Mapping;

namespace Nop.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [Column("Id", CanBeNull = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
    }
}
