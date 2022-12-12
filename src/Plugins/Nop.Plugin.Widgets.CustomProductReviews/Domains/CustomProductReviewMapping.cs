using Nop.Core;

namespace Nop.Plugin.Widgets.CustomProductReviews.Domains
{
    public partial class CustomProductReviewMapping : BaseEntity
    {
        public int Id { get; set; }
        public int ProductReviewId { get; set; }
        public int PictureId { get; set; }
        public int VideoId { get; set; }
        public int DisplayOrder { get; set; }

    }
}
