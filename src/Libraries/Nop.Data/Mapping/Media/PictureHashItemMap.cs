using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    /// <summary>
    /// Represents a picture hash item mapping configuration
    /// </summary>
    public partial class PictureHashItemMap : NopQueryTypeConfiguration<PictureHashItem>
    {
        #region Methods

        /// <summary>
        /// Configures the query type
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type</param>
        public override void Configure(QueryTypeBuilder<PictureHashItem> builder)
        {
            //add custom configuration
            this.PostConfigure(builder);
        }

        #endregion
    }
}