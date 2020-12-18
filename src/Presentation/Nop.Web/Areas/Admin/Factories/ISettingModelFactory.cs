using System.Threading.Tasks;
using Nop.Core.Domain.Gdpr;
using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory
    /// </summary>
    public partial interface ISettingModelFactory
    {
        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <returns>App settings model</returns>
        AppSettingsModel PrepareAppSettingsModel();

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <returns>Blog settings model</returns>
        Task<BlogSettingsModel> PrepareBlogSettingsModelAsync();

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <returns>Vendor settings model</returns>
        Task<VendorSettingsModel> PrepareVendorSettingsModelAsync();

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <returns>Forum settings model</returns>
        Task<ForumSettingsModel> PrepareForumSettingsModelAsync();

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <returns>News settings model</returns>
        Task<NewsSettingsModel> PrepareNewsSettingsModelAsync();

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <returns>Shipping settings model</returns>
        Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync();

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <returns>Tax settings model</returns>
        Task<TaxSettingsModel> PrepareTaxSettingsModelAsync();

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <returns>Catalog settings model</returns>
        Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync();

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel);

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        /// <returns>Reward points settings model</returns>
        Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync();

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <returns>Order settings model</returns>
        Task<OrderSettingsModel> PrepareOrderSettingsModelAsync();

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <returns>Shopping cart settings model</returns>
        Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync();

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        Task<MediaSettingsModel> PrepareMediaSettingsModelAsync();

        /// <summary>
        /// Prepare customer user settings model
        /// </summary>
        /// <returns>Customer user settings model</returns>
        Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync();

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <returns>GDPR settings model</returns>
        Task<GdprSettingsModel> PrepareGdprSettingsModelAsync();

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>GDPR consent list model</returns>
        Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel);

        /// <summary>
        /// Prepare GDPR consent model
        /// </summary>
        /// <param name="model">GDPR consent model</param>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>GDPR consent model</returns>
        Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false);

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync();

        /// <summary>
        /// Prepare product editor settings model
        /// </summary>
        /// <returns>Product editor settings model</returns>
        Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModelAsync();

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName);

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>Store scope configuration model</returns>
        Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync();
    }
}