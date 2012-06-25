
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        public int AvatarPictureSize { get; set; }
        public int ProductThumbPictureSize { get; set; }
        public int ProductDetailsPictureSize { get; set; }
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }
        public int ProductVariantPictureSize { get; set; }
        public int CategoryThumbPictureSize { get; set; }
        public int ManufacturerThumbPictureSize { get; set; }
        public int CartThumbPictureSize { get; set; }
        public int MiniCartThumbPictureSize { get; set; }
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        public bool DefaultPictureZoomEnabled { get; set; }

        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Geta or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }
    }
}