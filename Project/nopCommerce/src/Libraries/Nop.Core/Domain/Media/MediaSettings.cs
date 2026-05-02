using Nop.Core.Configuration;

namespace Nop.Core.Domain.Media;

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
    /// Picture size of product pictures for mini shopping cart box
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
    public bool AllowSvgUploads { get; set; }

    /// <summary>
    /// Maximum allowed picture size. If a larger picture is uploaded, then it'll be resized
    /// </summary>
    public int MaximumImageSize { get; set; }

    /// <summary>
    /// Gets or sets a default quality used for image generation
    /// </summary>
    public int DefaultImageQuality { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether single (thumbs/) or multiple (thumbs/001/ and thumbs/002/) directories will be used for picture thumbs
    /// </summary>
    public bool MultipleThumbDirectories { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we should use fast HASHBYTES (hash sum) database function to compare pictures when importing products
    /// </summary>
    public bool ImportProductImagesUsingHash { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we need to use absolute pictures path
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
    /// Gets or sets the product default image id. If 0, then default-image.png will be used
    /// </summary>
    public int ProductDefaultImageId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we need to reorient images automatically
    /// </summary>
    public bool AutoOrientImage { get; set; }

    /// <summary>
    /// Gets a path to the picture files
    /// </summary>
    public string PicturePath { get; set; }
}