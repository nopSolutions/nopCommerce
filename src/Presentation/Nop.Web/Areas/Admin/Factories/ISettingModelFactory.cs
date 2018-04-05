using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory
    /// </summary>
    public partial interface ISettingModelFactory
    {
        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <returns>Blog settings model</returns>
        BlogSettingsModel PrepareBlogSettingsModel();

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <returns>Vendor settings model</returns>
        VendorSettingsModel PrepareVendorSettingsModel();

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <returns>Forum settings model</returns>
        ForumSettingsModel PrepareForumSettingsModel();

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <returns>News settings model</returns>
        NewsSettingsModel PrepareNewsSettingsModel();

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <returns>Shipping settings model</returns>
        ShippingSettingsModel PrepareShippingSettingsModel();

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <returns>Tax settings model</returns>
        TaxSettingsModel PrepareTaxSettingsModel();

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <returns>Catalog settings model</returns>
        CatalogSettingsModel PrepareCatalogSettingsModel();

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option search model</returns>
        SortOptionSearchModel PrepareSortOptionSearchModel(SortOptionSearchModel searchModel);

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        SortOptionListModel PrepareSortOptionListModel(SortOptionSearchModel searchModel);

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        /// <returns>Reward points settings model</returns>
        RewardPointsSettingsModel PrepareRewardPointsSettingsModel();

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <returns>Order settings model</returns>
        OrderSettingsModel PrepareOrderSettingsModel();

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <returns>Shopping cart settings model</returns>
        ShoppingCartSettingsModel PrepareShoppingCartSettingsModel();

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        MediaSettingsModel PrepareMediaSettingsModel();

        /// <summary>
        /// Prepare customer user settings model
        /// </summary>
        /// <returns>Customer user settings model</returns>
        CustomerUserSettingsModel PrepareCustomerUserSettingsModel();

        /// <summary>
        /// Prepare address settings model
        /// </summary>
        /// <returns>Address settings model</returns>
        AddressSettingsModel PrepareAddressSettingsModel();

        /// <summary>
        /// Prepare customer settings model
        /// </summary>
        /// <returns>Customer settings model</returns>
        CustomerSettingsModel PrepareCustomerSettingsModel();

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>Date time settings model</returns>
        DateTimeSettingsModel PrepareDateTimeSettingsModel();

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>External authentication settings model</returns>
        ExternalAuthenticationSettingsModel PrepareExternalAuthenticationSettingsModel();

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        GeneralCommonSettingsModel PrepareGeneralCommonSettingsModel();

        /// <summary>
        /// Prepare store information settings model
        /// </summary>
        /// <returns>Store information settings model</returns>
        StoreInformationSettingsModel PrepareStoreInformationSettingsModel();

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>SEO settings model</returns>
        SeoSettingsModel PrepareSeoSettingsModel();

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>Security settings model</returns>
        SecuritySettingsModel PrepareSecuritySettingsModel();

        /// <summary>
        /// Prepare captcha settings model
        /// </summary>
        /// <returns>Captcha settings model</returns>
        CaptchaSettingsModel PrepareCaptchaSettingsModel();

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>PDF settings model</returns>
        PdfSettingsModel PreparePdfSettingsModel();

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>Localization settings model</returns>
        LocalizationSettingsModel PrepareLocalizationSettingsModel();

        /// <summary>
        /// Prepare full-text settings model
        /// </summary>
        /// <returns>Full-text settings model</returns>
        FullTextSettingsModel PrepareFullTextSettingsModel();

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>Admin area settings model</returns>
        AdminAreaSettingsModel PrepareAdminAreaSettingsModel();

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>Display default menu item settings model</returns>
        DisplayDefaultMenuItemSettingsModel PrepareDisplayDefaultMenuItemSettingsModel();

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>Display default footer item settings model</returns>
        DisplayDefaultFooterItemSettingsModel PrepareDisplayDefaultFooterItemSettingsModel();

        /// <summary>
        /// Prepare product editor settings model
        /// </summary>
        /// <returns>Product editor settings model</returns>
        ProductEditorSettingsModel PrepareProductEditorSettingsModel();

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        SettingSearchModel PrepareSettingSearchModel(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        SettingListModel PrepareSettingListModel(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        SettingModeModel PrepareSettingModeModel(string modeName);

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>Store scope configuration model</returns>
        StoreScopeConfigurationModel PrepareStoreScopeConfigurationModel();
    }
}