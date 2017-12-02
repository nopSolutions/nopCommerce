namespace Nop.Core.Domain.Media
{
    public partial class Thumbnail : BaseEntity
    {
        public int PictureId { get; set; }
        public int ThumbSize { get; set; }
        public string FileName { get; set; }
    }
}