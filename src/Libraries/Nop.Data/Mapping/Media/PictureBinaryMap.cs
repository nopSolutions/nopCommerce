using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PictureBinaryMap : NopEntityTypeConfiguration<PictureBinary>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<PictureBinary> builder)
        {
            builder.ToTable(nameof(PictureBinary));
            builder.HasKey(pictureBinary => pictureBinary.Id);

            builder.Property(pictureBinary => pictureBinary.BinaryData);

            builder.HasOne(pictureBinary => pictureBinary.Picture)
                .WithOne(picture => picture.PictureBinary)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }

        #endregion
    }
}