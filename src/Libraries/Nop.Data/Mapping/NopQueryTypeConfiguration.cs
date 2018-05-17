using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nop.Data.Mapping
{
    /// <summary>
    /// Represents base query type mapping configuration
    /// </summary>
    /// <typeparam name="TQuery">Query type type</typeparam>
    public abstract class NopQueryTypeConfiguration<TQuery> : IQueryTypeConfiguration<TQuery> where TQuery : class
    {
        #region Utilities

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query</param>
        protected virtual void PostConfigure(QueryTypeBuilder<TQuery> builder)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the query type
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type</param>
        public abstract void Configure(QueryTypeBuilder<TQuery> builder);

        #endregion
    }
}