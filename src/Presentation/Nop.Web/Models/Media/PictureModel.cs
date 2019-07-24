using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Media
{
    public partial class PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}