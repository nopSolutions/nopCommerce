using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product tag with count mapping configuration
    /// </summary>
    public partial class ProductTagWithCountMap : NopQueryTypeConfiguration<ProductTagWithCount>
    {
        #region Methods

        /// <summary>
        /// Configures the query type
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type</param>
        public override void Configure(QueryTypeBuilder<ProductTagWithCount> builder)
        {
            //add custom configuration
            this.PostConfigure(builder);
        }

        #endregion
    }
}