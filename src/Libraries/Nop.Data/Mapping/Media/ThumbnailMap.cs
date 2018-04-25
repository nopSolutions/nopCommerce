using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    public partial class ThumbnailMap : NopEntityTypeConfiguration<Thumbnail>
    {
        public ThumbnailMap()
        {
            this.ToTable("Thumb_Mapping");
            this.HasKey(p => p.Id);
            this.Property(p => p.PictureId);
            this.Property(p => p.ThumbSize);
            this.Property(p => p.FileName);
        }
    }
}