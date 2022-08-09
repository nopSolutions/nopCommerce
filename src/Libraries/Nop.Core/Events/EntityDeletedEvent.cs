namespace Nop.Core.Events
{
    /// <summary>
    /// A container for passing entities that have been deleted. This is not used for entities that are deleted logically via a bit column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class EntityDeletedEvent<T> where T : BaseEntity
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entity">Entity</param>
        public EntityDeletedEvent(T entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Entity
        /// </summary>
        public T Entity { get; }
    }
}
