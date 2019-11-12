using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class NopEntityTypeConfiguration<TEntity> : IMappingConfiguration where TEntity : BaseEntity
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        public abstract void Configure(EntityMappingBuilder<TEntity> builder);

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public void ApplyConfiguration(FluentMappingBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TEntity>();
            builder.HasPrimaryKey(entity => entity.Id).HasIdentity(entity => entity.Id);

            Configure(builder);
        }

        #endregion
    }
}