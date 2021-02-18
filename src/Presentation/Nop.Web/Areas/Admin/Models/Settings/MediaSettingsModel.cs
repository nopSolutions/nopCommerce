using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a media settings model
    /// </summary>
    public partial record MediaSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }
        public bool AvatarPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ProductThumbPictureSize")]
        public int ProductThumbPictureSize { get; set; }
        public bool ProductThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ProductDetailsPictureSize")]
        public int ProductDetailsPictureSize { get; set; }
        public bool ProductDetailsPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ProductThumbPictureSizeOnProductDetailsPage")]
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }
        public bool ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedProductPictureSize")]
        public int AssociatedProductPictureSize { get; set; }
        public bool AssociatedProductPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }
        public bool CategoryThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ManufacturerThumbPictureSize")]
        public int ManufacturerThumbPictureSize { get; set; }
        public bool ManufacturerThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.VendorThumbPictureSize")]
        public int VendorThumbPictureSize { get; set; }
        public bool VendorThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }
        public bool CartThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }
        public bool MiniCartThumbPictureSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }
        public bool MaximumImageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MultipleThumbDirectories")]
        public bool MultipleThumbDirectories { get; set; }
        public bool MultipleThumbDirectories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.DefaultImageQuality")]
        public int DefaultImageQuality { get; set; }
        public bool DefaultImageQuality_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ImportProductImagesUsingHash")]
        public bool ImportProductImagesUsingHash { get; set; }
        public bool ImportProductImagesUsingHash_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }
        public bool DefaultPictureZoomEnabled_OverrideForStore { get; set; }

        #endregion
    }
}