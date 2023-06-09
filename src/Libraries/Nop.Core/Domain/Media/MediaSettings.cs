using Nop.Core.Configuration;

namespace Nop.Core.Domain.Media
{
    /// <summary>
    /// Media settings
    /// </summary>
    public partial class MediaSettings : ISettings
    {
        /// <summary>
        /// Picture size of customer avatars (if enabled)
        /// </summary>
        public int AvatarPictureSize { get; set; }

        /// <summary>
        /// Picture size of product picture thumbs displayed on catalog pages (e.g. category details page)
        /// </summary>
        public int ProductThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of the main product picture displayed on the product details page
        /// </summary>
        public int ProductDetailsPictureSize { get; set; }

        /// <summary>
        /// Picture size of the product picture thumbs displayed on the product details page
        /// </summary>
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }

        /// <summary>
        /// Picture size of the associated product picture
        /// </summary>
        public int AssociatedProductPictureSize { get; set; }

        /// <summary>
        /// Picture size of category pictures
        /// </summary>
        public int CategoryThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of manufacturer pictures
        /// </summary>
        public int ManufacturerThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of vendor pictures
        /// </summary>
        public int VendorThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of product pictures on the shopping cart page
        /// </summary>
        public int CartThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of product pictures on the order details page
        /// </summary>
        public int OrderThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of product pictures for minishipping cart box
        /// </summary>
        public int MiniCartThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of product pictures for autocomplete search box
        /// </summary>
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        /// <summary>
        /// Picture size of image squares on a product details page (used with "image squares" attribute type
        /// </summary>
        public int ImageSquarePictureSize { get; set; }

        /// <summary>
        /// A value indicating whether picture zoom is enabled
        /// </summary>
        public bool DefaultPictureZoomEnabled { get; set; }

        /// <summary>
        /// A value indicating whether to allow uploading of SVG files in admin area
        /// </summary>
        public bool AllowSVGUploads { get; set; }

        /// <summary>
        /// Maximum allowed picture size. If a larger picture is uploaded, then it'll be resized
        /// </summary>
        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Gets or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should use fast HASHBYTES (hash sum) database function to compare pictures when importing products
        /// </summary>
        public bool ImportProductImagesUsingHash { get; set; }

        /// <summary>
        /// Gets or sets Azure CacheControl header (e.g. "max-age=3600, public")
        /// </summary>
        /// <remarks>
        /// max-age=[seconds]     — specifies the maximum amount of time that a representation will be considered fresh. Similar to Expires, this directive is relative to the time of the request, rather than absolute. [seconds] is the number of seconds from the time of the request you wish the representation to be fresh for.
        /// s-maxage=[seconds]    — similar to max-age, except that it only applies to shared (e.g., proxy) caches.
        /// public                — marks authenticated responses as cacheable; normally, if HTTP authentication is required, responses are automatically private.
        /// private               — allows caches that are specific to one user (e.g., in a browser) to store the response; shared caches (e.g., in a proxy) may not.
        /// no-cache              — forces caches to submit the request to the origin server for validation before releasing a cached copy, every time. This is useful to assure that authentication is respected (in combination with public), or to maintain rigid freshness, without sacrificing all of the benefits of caching.
        /// no-store              — instructs caches not to keep a copy of the representation under any conditions.
        /// must-revalidate       — tells caches that they must obey any freshness information you give them about a representation. HTTP allows caches to serve stale representations under special conditions; by specifying this header, you’re telling the cache that you want it to strictly follow your rules.
        /// proxy-revalidate      — similar to must-revalidate, except that it only applies to proxy caches.
        /// </remarks>
        public string AzureCacheControlHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need to use absolute pictures path
        /// </summary>
        public bool UseAbsoluteImagePath { get; set; }

        /// <summary>
        /// Gets or sets the value to specify a policy list for embedded content
        /// </summary>
        public string VideoIframeAllow { get; set; }

        /// <summary>
        /// Gets or sets the width of the frame in CSS pixels
        /// </summary>
        public int VideoIframeWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame in CSS pixels
        /// </summary>
        public int VideoIframeHeight { get; set; }

        /// <summary>
        /// Gets or sets the product default image id. If 0, then wwwroot/images/default-image.png will be used
        /// </summary>
        public int ProductDefaultImageId { get; set; }
    }
}