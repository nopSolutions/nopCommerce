using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<PictureBinary> builder)
        {
            builder.HasTableName(nameof(PictureBinary));

            builder.Property(picturebinary => picturebinary.BinaryData);
            builder.Property(picturebinary => picturebinary.PictureId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(pictureBinary => pictureBinary.Picture)
            //    .WithOne(picture => picture.PictureBinary)
            //    .HasForeignKey<PictureBinary>(pictureBinary => pictureBinary.PictureId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}