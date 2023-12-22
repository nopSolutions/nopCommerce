using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a display default footer item settings model
/// </summary>
public partial record DisplayDefaultFooterItemSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem")]
    public bool DisplaySitemapFooterItem { get; set; }
    public bool DisplaySitemapFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem")]
    public bool DisplayContactUsFooterItem { get; set; }
    public bool DisplayContactUsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem")]
    public bool DisplayProductSearchFooterItem { get; set; }
    public bool DisplayProductSearchFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem")]
    public bool DisplayNewsFooterItem { get; set; }
    public bool DisplayNewsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem")]
    public bool DisplayBlogFooterItem { get; set; }
    public bool DisplayBlogFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem")]
    public bool DisplayForumsFooterItem { get; set; }
    public bool DisplayForumsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem")]
    public bool DisplayRecentlyViewedProductsFooterItem { get; set; }
    public bool DisplayRecentlyViewedProductsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem")]
    public bool DisplayCompareProductsFooterItem { get; set; }
    public bool DisplayCompareProductsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem")]
    public bool DisplayNewProductsFooterItem { get; set; }
    public bool DisplayNewProductsFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem")]
    public bool DisplayCustomerInfoFooterItem { get; set; }
    public bool DisplayCustomerInfoFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem")]
    public bool DisplayCustomerOrdersFooterItem { get; set; }
    public bool DisplayCustomerOrdersFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem")]
    public bool DisplayCustomerAddressesFooterItem { get; set; }
    public bool DisplayCustomerAddressesFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem")]
    public bool DisplayShoppingCartFooterItem { get; set; }
    public bool DisplayShoppingCartFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem")]
    public bool DisplayWishlistFooterItem { get; set; }
    public bool DisplayWishlistFooterItem_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem")]
    public bool DisplayApplyVendorAccountFooterItem { get; set; }
    public bool DisplayApplyVendorAccountFooterItem_OverrideForStore { get; set; }

    #endregion
}