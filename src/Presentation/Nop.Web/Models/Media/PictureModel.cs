using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Media
{
    public partial record PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
        public string HoverImageUrl { get; set; }

        public string HoverThumbImageUrl { get; set; }

        public string HoverFullSizeImageUrl { get; set; }

        public string HoverTitle { get; set; }

        public string HoverAlternateText { get; set; }
    }
}