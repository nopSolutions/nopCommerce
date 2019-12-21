using Microsoft.EntityFrameworkCore;
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
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<PictureHashItem> builder)
        {
            builder.ToTable(nameof(PictureHashItem));
            builder.HasNoKey();

            base.Configure(builder);
        }

        #endregion
    }
}