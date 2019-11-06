namespace Nop.Data
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class NopEntityTypeConfiguration<TEntity> : IMappingConfiguration
    {
        #region Utilities

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code
        /// </summary>
        protected virtual void PostConfigure()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        public virtual void Configure()
        {
            //add custom configuration
            PostConfigure();
        }

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        public virtual void ApplyConfiguration()
        {
            
        }

        #endregion
    }
}