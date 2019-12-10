using LinqToDB.Mapping;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    /// <summary>
    /// Represents a picture mapping configuration
    /// </summary>
    public partial class PictureMap : NopEntityTypeConfiguration<Picture>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Picture> builder)
        {
            builder.HasTableName(nameof(Picture));
            
            builder.Property(picture => picture.MimeType).HasLength(40).IsNullable(false);
            builder.Property(picture => picture.SeoFilename).HasLength(300);
            builder.Property(picture => picture.AltAttribute);
            builder.Property(picture => picture.TitleAttribute);
            builder.Property(picture => picture.IsNew);
            builder.Property(picture => picture.VirtualPath);
        }

        #endregion
    }
}