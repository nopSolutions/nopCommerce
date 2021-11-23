using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory implementation
    /// </summary>
    public partial class SettingModelFactory : ISettingModelFactory
    {
        #region Fields

        protected AppSettings AppSettings { get; }
        protected CurrencySettings CurrencySettings { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAddressAttributeModelFactory AddressAttributeModelFactory { get; }
        protected IAddressService AddressService { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerAttributeModelFactory CustomerAttributeModelFactory { get; }
        protected INopDataProvider DataProvider { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGdprService GdprService { get; }
        protected ILocalizedModelFactory LocalizedModelFactory { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPictureService PictureService { get; }
        protected IReturnRequestModelFactory ReturnRequestModelFactory { get; }
        protected IReviewTypeModelFactory ReviewTypeModelFactory { get; }
        protected ISettingService SettingService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreService StoreService { get; }
        protected IThemeProvider ThemeProvider { get; }
        protected IVendorAttributeModelFactory VendorAttributeModelFactory { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public SettingModelFactory(AppSettings appSettings,
            CurrencySettings currencySettings,
            IAddressModelFactory addressModelFactory,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            ICustomerAttributeModelFactory customerAttributeModelFactory,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IGdprService gdprService,
            ILocalizedModelFactory localizedModelFactory,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IReturnRequestModelFactory returnRequestModelFactory,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IThemeProvider themeProvider,
            IVendorAttributeModelFactory vendorAttributeModelFactory,
            IReviewTypeModelFactory reviewTypeModelFactory,
            IWorkContext workContext)
        {
            AppSettings = appSettings;
            CurrencySettings = currencySettings;
            AddressModelFactory = addressModelFactory;
            AddressAttributeModelFactory = addressAttributeModelFactory;
            AddressService = addressService;
            BaseAdminModelFactory = baseAdminModelFactory;
            CurrencyService = currencyService;
            CustomerAttributeModelFactory = customerAttributeModelFactory;
            DataProvider = dataProvider;
            DateTimeHelper = dateTimeHelper;
            GdprService = gdprService;
            LocalizedModelFactory = localizedModelFactory;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            PictureService = pictureService;
            ReturnRequestModelFactory = returnRequestModelFactory;
            SettingService = settingService;
            StoreContext = storeContext;
            StoreService = storeService;
            ThemeProvider = themeProvider;
            VendorAttributeModelFactory = vendorAttributeModelFactory;
            ReviewTypeModelFactory = reviewTypeModelFactory;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare store theme models
        /// </summary>
        /// <param name="models">List of store theme models</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await SettingService.LoadSettingAsync<StoreInformationSettings>(storeId);

            //get available themes
            var availableThemes = await ThemeProvider.GetThemesAsync();
            foreach (var theme in availableThemes)
            {
                models.Add(new StoreInformationSettingsModel.ThemeModel
                {
                    FriendlyName = theme.FriendlyName,
                    SystemName = theme.SystemName,
                    PreviewImageUrl = theme.PreviewImageUrl,
                    PreviewText = theme.PreviewText,
                    SupportRtl = theme.SupportRtl,
                    Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                });
            }
        }

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sort option search model
        /// </returns>
        protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare GDPR consent search model
        /// </summary>
        /// <param name="searchModel">GDPR consent search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent search model
        /// </returns>
        protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare address settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address settings model
        /// </returns>
        protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var addressSettings = await SettingService.LoadSettingAsync<AddressSettings>(storeId);

            //fill in model values from the entity
            var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare customer settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer settings model
        /// </returns>
        protected virtual async Task<CustomerSettingsModel> PrepareCustomerSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var customerSettings = await SettingService.LoadSettingAsync<CustomerSettings>(storeId);

            //fill in model values from the entity
            var model = customerSettings.ToSettingsModel<CustomerSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare multi-factor authentication settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multiFactorAuthenticationSettingsModel
        /// </returns>
        protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var multiFactorAuthenticationSettings = await SettingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

            return model;

        }

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the date time settings model
        /// </returns>
        protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var dateTimeSettings = await SettingService.LoadSettingAsync<DateTimeSettings>(storeId);

            //fill in model values from the entity
            var model = new DateTimeSettingsModel
            {
                AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone
            };

            //fill in additional values (not existing in the entity)
            model.DefaultStoreTimeZoneId = DateTimeHelper.DefaultStoreTimeZone.Id;

            //prepare available time zones
            await BaseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

            return model;
        }

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the external authentication settings model
        /// </returns>
        protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var externalAuthenticationSettings = await SettingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeId);

            //fill in model values from the entity
            var model = new ExternalAuthenticationSettingsModel
            {
                AllowCustomersToRemoveAssociations = externalAuthenticationSettings.AllowCustomersToRemoveAssociations
            };

            return model;
        }

        /// <summary>
        /// Prepare store information settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store information settings model
        /// </returns>
        protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var storeInformationSettings = await SettingService.LoadSettingAsync<StoreInformationSettings>(storeId);
            var commonSettings = await SettingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new StoreInformationSettingsModel
            {
                StoreClosed = storeInformationSettings.StoreClosed,
                DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
                AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme,
                LogoPictureId = storeInformationSettings.LogoPictureId,
                DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
                FacebookLink = storeInformationSettings.FacebookLink,
                TwitterLink = storeInformationSettings.TwitterLink,
                YoutubeLink = storeInformationSettings.YoutubeLink,
                SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
                UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
                PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
            };

            //prepare available themes
            await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.StoreClosed_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
            model.DefaultStoreTheme_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
            model.AllowCustomerToSelectTheme_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeId);
            model.LogoPictureId_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
            model.DisplayEuCookieLawWarning_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
            model.FacebookLink_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
            model.TwitterLink_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
            model.YoutubeLink_OverrideForStore = await SettingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
            model.SubjectFieldOnContactUsForm_OverrideForStore = await SettingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
            model.UseSystemEmailForContactUsForm_OverrideForStore = await SettingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
            model.PopupForTermsOfServiceLinks_OverrideForStore = await SettingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

            return model;
        }

        /// <summary>
        /// Prepare Sitemap settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap settings model
        /// </returns>
        protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var sitemapSettings = await SettingService.LoadSettingAsync<SitemapSettings>(storeId);

            //fill in model values from the entity
            var model = new SitemapSettingsModel
            {
                SitemapEnabled = sitemapSettings.SitemapEnabled,
                SitemapPageSize = sitemapSettings.SitemapPageSize,
                SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
                SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
                SitemapIncludeProducts = sitemapSettings.SitemapIncludeProducts,
                SitemapIncludeProductTags = sitemapSettings.SitemapIncludeProductTags,
                SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
                SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
                SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.SitemapEnabled_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
            model.SitemapPageSize_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
            model.SitemapIncludeCategories_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
            model.SitemapIncludeManufacturers_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
            model.SitemapIncludeProducts_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
            model.SitemapIncludeProductTags_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
            model.SitemapIncludeBlogPosts_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
            model.SitemapIncludeNews_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeNews, storeId);
            model.SitemapIncludeTopics_OverrideForStore = await SettingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

            return model;
        }

        /// <summary>
        /// Prepare minification settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the minification settings model
        /// </returns>
        protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var minificationSettings = await SettingService.LoadSettingAsync<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new MinificationSettingsModel
            {
                EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
                EnableJsBundling = minificationSettings.EnableJsBundling,
                EnableCssBundling = minificationSettings.EnableCssBundling,
                UseResponseCompression = minificationSettings.UseResponseCompression
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.EnableHtmlMinification_OverrideForStore = await SettingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
            model.EnableJsBundling_OverrideForStore = await SettingService.SettingExistsAsync(minificationSettings, x => x.EnableJsBundling, storeId);
            model.EnableCssBundling_OverrideForStore = await SettingService.SettingExistsAsync(minificationSettings, x => x.EnableCssBundling, storeId);
            model.UseResponseCompression_OverrideForStore = await SettingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

            return model;
        }

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sEO settings model
        /// </returns>
        protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var seoSettings = await SettingService.LoadSettingAsync<SeoSettings>(storeId);

            //fill in model values from the entity
            var model = new SeoSettingsModel
            {
                PageTitleSeparator = seoSettings.PageTitleSeparator,
                PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
                PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
                HomepageTitle = seoSettings.HomepageTitle,
                HomepageDescription = seoSettings.HomepageDescription,
                DefaultTitle = seoSettings.DefaultTitle,
                DefaultMetaKeywords = seoSettings.DefaultMetaKeywords,
                DefaultMetaDescription = seoSettings.DefaultMetaDescription,
                GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
                ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
                CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
                WwwRequirement = (int)seoSettings.WwwRequirement,
                WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

                TwitterMetaTags = seoSettings.TwitterMetaTags,
                OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
                CustomHeadTags = seoSettings.CustomHeadTags,
                MicrodataEnabled = seoSettings.MicrodataEnabled
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.PageTitleSeparator_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
            model.PageTitleSeoAdjustment_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
            model.DefaultTitle_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.DefaultTitle, storeId);
            model.HomepageTitle_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.HomepageTitle, storeId);
            model.HomepageDescription_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.HomepageDescription, storeId);
            model.DefaultMetaKeywords_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.DefaultMetaKeywords, storeId);
            model.DefaultMetaDescription_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.DefaultMetaDescription, storeId);
            model.GenerateProductMetaDescription_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.GenerateProductMetaDescription, storeId);
            model.ConvertNonWesternChars_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
            model.CanonicalUrlsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
            model.WwwRequirement_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
            model.TwitterMetaTags_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
            model.OpenGraphMetaTags_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
            model.CustomHeadTags_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
            model.MicrodataEnabled_OverrideForStore = await SettingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the security settings model
        /// </returns>
        protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var securitySettings = await SettingService.LoadSettingAsync<SecuritySettings>(storeId);

            //fill in model values from the entity
            var model = new SecuritySettingsModel
            {
                EncryptionKey = securitySettings.EncryptionKey,
                HoneypotEnabled = securitySettings.HoneypotEnabled
            };

            //fill in additional values (not existing in the entity)
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

            return model;
        }

        /// <summary>
        /// Prepare captcha settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the captcha settings model
        /// </returns>
        protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var captchaSettings = await SettingService.LoadSettingAsync<CaptchaSettings>(storeId);

            //fill in model values from the entity
            var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

            model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            model.Enabled_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
            model.ShowOnLoginPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
            model.ShowOnRegistrationPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
            model.ShowOnContactUsPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
            model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
            model.ShowOnEmailProductToFriendPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
            model.ShowOnBlogCommentPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
            model.ShowOnNewsCommentPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
            model.ShowOnProductReviewPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
            model.ShowOnApplyVendorPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
            model.ShowOnForgotPasswordPage_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
            model.ShowOnForum_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
            model.ReCaptchaPublicKey_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
            model.ReCaptchaPrivateKey_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
            model.CaptchaType_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
            model.ReCaptchaV3ScoreThreshold_OverrideForStore = await SettingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

            return model;
        }

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pDF settings model
        /// </returns>
        protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var pdfSettings = await SettingService.LoadSettingAsync<PdfSettings>(storeId);

            //fill in model values from the entity
            var model = new PdfSettingsModel
            {
                LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
                LogoPictureId = pdfSettings.LogoPictureId,
                DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
                InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
                InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.LetterPageSizeEnabled_OverrideForStore = await SettingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
            model.LogoPictureId_OverrideForStore = await SettingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
            model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await SettingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
            model.InvoiceFooterTextColumn1_OverrideForStore = await SettingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
            model.InvoiceFooterTextColumn2_OverrideForStore = await SettingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

            return model;
        }

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localization settings model
        /// </returns>
        protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var localizationSettings = await SettingService.LoadSettingAsync<LocalizationSettings>(storeId);

            //fill in model values from the entity
            var model = new LocalizationSettingsModel
            {
                UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
                SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
                AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
                LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
                LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
                LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
            };

            return model;
        }

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the admin area settings model
        /// </returns>
        protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var adminAreaSettings = await SettingService.LoadSettingAsync<AdminAreaSettings>(storeId);

            //fill in model values from the entity
            var model = new AdminAreaSettingsModel
            {
                UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
            };

            //fill in overridden values
            if (storeId > 0)
            {
                model.UseRichEditorInMessageTemplates_OverrideForStore = await SettingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
            }

            return model;
        }

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the display default menu item settings model
        /// </returns>
        protected virtual async Task<DisplayDefaultMenuItemSettingsModel> PrepareDisplayDefaultMenuItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultMenuItemSettings = await SettingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultMenuItemSettingsModel
            {
                DisplayHomepageMenuItem = displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewProductsMenuItem = displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
                DisplayProductSearchMenuItem = displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
                DisplayCustomerInfoMenuItem = displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
                DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayHomepageMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayHomepageMenuItem, storeId);
            model.DisplayNewProductsMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, storeId);
            model.DisplayProductSearchMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, storeId);
            model.DisplayCustomerInfoMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, storeId);
            model.DisplayBlogMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeId);
            model.DisplayForumsMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeId);
            model.DisplayContactUsMenuItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the display default footer item settings model
        /// </returns>
        protected virtual async Task<DisplayDefaultFooterItemSettingsModel> PrepareDisplayDefaultFooterItemSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var displayDefaultFooterItemSettings = await SettingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>(storeId);

            //fill in model values from the entity
            var model = new DisplayDefaultFooterItemSettingsModel
            {
                DisplaySitemapFooterItem = displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
                DisplayCustomerInfoFooterItem = displayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem,
                DisplayCustomerOrdersFooterItem = displayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem,
                DisplayCustomerAddressesFooterItem = displayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem,
                DisplayShoppingCartFooterItem = displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplaySitemapFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplaySitemapFooterItem, storeId);
            model.DisplayContactUsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayContactUsFooterItem, storeId);
            model.DisplayProductSearchFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayProductSearchFooterItem, storeId);
            model.DisplayNewsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewsFooterItem, storeId);
            model.DisplayBlogFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayBlogFooterItem, storeId);
            model.DisplayForumsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayForumsFooterItem, storeId);
            model.DisplayRecentlyViewedProductsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayRecentlyViewedProductsFooterItem, storeId);
            model.DisplayCompareProductsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCompareProductsFooterItem, storeId);
            model.DisplayNewProductsFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayNewProductsFooterItem, storeId);
            model.DisplayCustomerInfoFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCustomerInfoFooterItem, storeId);
            model.DisplayCustomerOrdersFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCustomerOrdersFooterItem, storeId);
            model.DisplayCustomerAddressesFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayCustomerAddressesFooterItem, storeId);
            model.DisplayShoppingCartFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayShoppingCartFooterItem, storeId);
            model.DisplayWishlistFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayWishlistFooterItem, storeId);
            model.DisplayApplyVendorAccountFooterItem_OverrideForStore = await SettingService.SettingExistsAsync(displayDefaultFooterItemSettings, x => x.DisplayApplyVendorAccountFooterItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare setting model to add
        /// </summary>
        /// <param name="model">Setting model to add</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <param name="model">AppSettings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the app settings model
        /// </returns>
        public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
        {
            model ??= new AppSettingsModel
            {
                CacheConfigModel = AppSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
                HostingConfigModel = AppSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
                DistributedCacheConfigModel = AppSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
                AzureBlobConfigModel = AppSettings.Get<AzureBlobConfig>().ToConfigModel<AzureBlobConfigModel>(),
                InstallationConfigModel = AppSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
                PluginConfigModel = AppSettings.Get<PluginConfig>().ToConfigModel<PluginConfigModel>(),
                CommonConfigModel = AppSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
                DataConfigModel = AppSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>()
            };

            model.DistributedCacheConfigModel.DistributedCacheTypeValues = await AppSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

            model.DataConfigModel.DataProviderTypeValues = await AppSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

            //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
            //we are forced to adjust our internal model naming to this convention in this check.
            model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
                                                where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
                                                from pp in property.PropertyType.GetProperties()
                                                where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
                                                select $"{property.Name}_{pp.Name}");
            return model;
        }

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <param name="model">Blog settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog settings model
        /// </returns>
        public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var blogSettings = await SettingService.LoadSettingAsync<BlogSettings>(storeId);

            //fill in model values from the entity
            model ??= blogSettings.ToSettingsModel<BlogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.Enabled, storeId);
            model.PostsPageSize_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.PostsPageSize, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewBlogComments_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
            model.NumberOfTags_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.NumberOfTags, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.ShowHeaderRssUrl, storeId);
            model.BlogCommentsMustBeApproved_OverrideForStore = await SettingService.SettingExistsAsync(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <param name="model">Vendor settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor settings model
        /// </returns>
        public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModelAsync(VendorSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var vendorSettings = await SettingService.LoadSettingAsync<VendorSettings>(storeId);

            //fill in model values from the entity
            model ??= vendorSettings.ToSettingsModel<VendorSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.VendorsBlockItemsToDisplay_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
                model.ShowVendorOnProductDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeId);
                model.ShowVendorOnOrderDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
                model.AllowCustomersToContactVendors_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToContactVendors, storeId);
                model.AllowCustomersToApplyForVendorAccount_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeId);
                model.TermsOfServiceEnabled_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
                model.AllowSearchByVendor_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.AllowSearchByVendor, storeId);
                model.AllowVendorsToEditInfo_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
                model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
                model.MaximumProductNumber_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.MaximumProductNumber, storeId);
                model.AllowVendorsToImportProducts_OverrideForStore = await SettingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToImportProducts, storeId);
            }

            //prepare nested search model
            await VendorAttributeModelFactory.PrepareVendorAttributeSearchModelAsync(model.VendorAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <param name="model">Forum settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum settings model
        /// </returns>
        public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var forumSettings = await SettingService.LoadSettingAsync<ForumSettings>(storeId);

            //fill in model values from the entity
            model ??= forumSettings.ToSettingsModel<ForumSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.ForumsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
            model.RelativeDateTimeFormattingEnabled_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
            model.ShowCustomersPostCount_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ShowCustomersPostCount, storeId);
            model.AllowGuestsToCreatePosts_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
            model.AllowGuestsToCreateTopics_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
            model.AllowCustomersToEditPosts_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToEditPosts, storeId);
            model.AllowCustomersToDeletePosts_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToDeletePosts, storeId);
            model.AllowPostVoting_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
            model.MaxVotesPerDay_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
            model.AllowCustomersToManageSubscriptions_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeId);
            model.TopicsPageSize_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
            model.PostsPageSize_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
            model.ForumEditor_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
            model.SignaturesEnabled_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
            model.AllowPrivateMessages_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.AllowPrivateMessages, storeId);
            model.ShowAlertForPM_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ShowAlertForPM, storeId);
            model.NotifyAboutPrivateMessages_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
            model.ActiveDiscussionsFeedEnabled_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
            model.ActiveDiscussionsFeedCount_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
            model.ForumFeedsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
            model.ForumFeedCount_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
            model.SearchResultsPageSize_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
            model.ActiveDiscussionsPageSize_OverrideForStore = await SettingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <param name="model">News settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the news settings model
        /// </returns>
        public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var newsSettings = await SettingService.LoadSettingAsync<NewsSettings>(storeId);

            //fill in model values from the entity
            model ??= newsSettings.ToSettingsModel<NewsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.Enabled, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewNewsComments_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
            model.ShowNewsOnMainPage_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.ShowNewsOnMainPage, storeId);
            model.MainPageNewsCount_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.MainPageNewsCount, storeId);
            model.NewsArchivePageSize_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.NewsArchivePageSize, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.ShowHeaderRssUrl, storeId);
            model.NewsCommentsMustBeApproved_OverrideForStore = await SettingService.SettingExistsAsync(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <param name="model">Shipping settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping settings model
        /// </returns>
        public virtual async Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync(ShippingSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var shippingSettings = await SettingService.LoadSettingAsync<ShippingSettings>(storeId);

            //fill in model values from the entity
            model ??= shippingSettings.ToSettingsModel<ShippingSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.SortShippingValues = await shippingSettings.ShippingSorting.ToSelectListAsync();

            //fill in overridden values
            if (storeId > 0)
            {
                model.ShipToSameAddress_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.ShipToSameAddress, storeId);
                model.AllowPickupInStore_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.AllowPickupInStore, storeId);
                model.DisplayPickupPointsOnMap_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.DisplayPickupPointsOnMap, storeId);
                model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, storeId);
                model.GoogleMapsApiKey_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.GoogleMapsApiKey, storeId);
                model.UseWarehouseLocation_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.UseWarehouseLocation, storeId);
                model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeId);
                model.FreeShippingOverXEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXEnabled, storeId);
                model.FreeShippingOverXValue_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXValue, storeId);
                model.FreeShippingOverXIncludingTax_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeId);
                model.EstimateShippingCartPageEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCartPageEnabled, storeId);
                model.EstimateShippingProductPageEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingProductPageEnabled, storeId);
                model.EstimateShippingCityNameEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCityNameEnabled, storeId);
                model.DisplayShipmentEventsToCustomers_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeId);
                model.DisplayShipmentEventsToStoreOwner_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeId);
                model.HideShippingTotal_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.HideShippingTotal, storeId);
                model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeId);
                model.ConsiderAssociatedProductsDimensions_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.ConsiderAssociatedProductsDimensions, storeId);
                model.ShippingOriginAddress_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.ShippingOriginAddressId, storeId);
                model.ShippingSorting_OverrideForStore = await SettingService.SettingExistsAsync(shippingSettings, x => x.ShippingSorting, storeId);
            }

            //prepare shipping origin address
            var originAddress = await AddressService.GetAddressByIdAsync(shippingSettings.ShippingOriginAddressId);
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel(model.ShippingOriginAddress);
            await AddressModelFactory.PrepareAddressModelAsync(model.ShippingOriginAddress, originAddress);
            model.ShippingOriginAddress.ZipPostalCodeRequired = true;

            return model;
        }

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <param name="model">Tax settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax settings model
        /// </returns>
        public virtual async Task<TaxSettingsModel> PrepareTaxSettingsModelAsync(TaxSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var taxSettings = await SettingService.LoadSettingAsync<TaxSettings>(storeId);

            //fill in model values from the entity
            model ??= taxSettings.ToSettingsModel<TaxSettingsModel>();
            model.TaxBasedOnValues = await taxSettings.TaxBasedOn.ToSelectListAsync();
            model.TaxDisplayTypeValues = await taxSettings.TaxDisplayType.ToSelectListAsync();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.PricesIncludeTax_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.PricesIncludeTax, storeId);
                model.AllowCustomersToSelectTaxDisplayType_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeId);
                model.TaxDisplayType_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.TaxDisplayType, storeId);
                model.DisplayTaxSuffix_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxSuffix, storeId);
                model.DisplayTaxRates_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxRates, storeId);
                model.HideZeroTax_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.HideZeroTax, storeId);
                model.HideTaxInOrderSummary_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.HideTaxInOrderSummary, storeId);
                model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeId);
                model.DefaultTaxCategoryId_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxCategoryId, storeId);
                model.TaxBasedOn_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOn, storeId);
                model.TaxBasedOnPickupPointAddress_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeId);
                model.DefaultTaxAddress_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxAddressId, storeId);
                model.ShippingIsTaxable_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.ShippingIsTaxable, storeId);
                model.ShippingPriceIncludesTax_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.ShippingPriceIncludesTax, storeId);
                model.ShippingTaxClassId_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.ShippingTaxClassId, storeId);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeId);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeId);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeId);
                model.EuVatEnabled_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabled, storeId);
                model.EuVatShopCountryId_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatShopCountryId, storeId);
                model.EuVatAllowVatExemption_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatAllowVatExemption, storeId);
                model.EuVatUseWebService_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatUseWebService, storeId);
                model.EuVatAssumeValid_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatAssumeValid, storeId);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = await SettingService.SettingExistsAsync(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeId);
            }

            //prepare available tax categories
            await BaseAdminModelFactory.PrepareTaxCategoriesAsync(model.TaxCategories);

            //prepare available EU VAT countries
            await BaseAdminModelFactory.PrepareCountriesAsync(model.EuVatShopCountries);

            //prepare default tax address
            var defaultAddress = await AddressService.GetAddressByIdAsync(taxSettings.DefaultTaxAddressId);
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel(model.DefaultTaxAddress);
            await AddressModelFactory.PrepareAddressModelAsync(model.DefaultTaxAddress, defaultAddress);
            model.DefaultTaxAddress.ZipPostalCodeRequired = true;

            return model;
        }

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <param name="model">Catalog settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the catalog settings model
        /// </returns>
        public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync(CatalogSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await SettingService.LoadSettingAsync<CatalogSettings>(storeId);

            //fill in model values from the entity
            model ??= catalogSettings.ToSettingsModel<CatalogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.AttributeValueOutOfStockDisplayTypes = await catalogSettings.AttributeValueOutOfStockDisplayType.ToSelectListAsync();
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.ViewMode.Grid"),
                Value = "grid"
            });
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.ViewMode.List"),
                Value = "list"
            });

            //fill in overridden values
            if (storeId > 0)
            {
                model.AllowViewUnpublishedProductPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeId);
                model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeId);
                model.ShowSkuOnProductDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeId);
                model.ShowSkuOnCatalogPages_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
                model.ShowManufacturerPartNumber_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
                model.ShowGtin_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowGtin, storeId);
                model.ShowFreeShippingNotification_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
                model.AllowProductSorting_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AllowProductSorting, storeId);
                model.AllowProductViewModeChanging_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AllowProductViewModeChanging, storeId);
                model.DefaultViewMode_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DefaultViewMode, storeId);
                model.ShowProductsFromSubcategories_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowProductsFromSubcategories, storeId);
                model.ShowCategoryProductNumber_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumber, storeId);
                model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeId);
                model.CategoryBreadcrumbEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
                model.ShowShareButton_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowShareButton, storeId);
                model.PageShareCode_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.PageShareCode, storeId);
                model.ProductReviewsMustBeApproved_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsMustBeApproved, storeId);
                model.OneReviewPerProductFromCustomer_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.OneReviewPerProductFromCustomer, storeId);
                model.AllowAnonymousUsersToReviewProduct_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeId);
                model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeId);
                model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeId);
                model.NotifyCustomerAboutProductReviewReply_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NotifyCustomerAboutProductReviewReply, storeId);
                model.EmailAFriendEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.EmailAFriendEnabled, storeId);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
                model.RecentlyViewedProductsNumber_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsNumber, storeId);
                model.RecentlyViewedProductsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeId);
                model.NewProductsNumber_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NewProductsNumber, storeId);
                model.NewProductsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NewProductsEnabled, storeId);
                model.CompareProductsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.CompareProductsEnabled, storeId);
                model.ShowBestsellersOnHomepage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
                model.SearchPageProductsPerPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPageProductsPerPage, storeId);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeId);
                model.SearchPagePriceRangeFiltering_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceRangeFiltering, storeId);
                model.SearchPagePriceFrom_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceFrom, storeId);
                model.SearchPagePriceTo_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceTo, storeId);
                model.SearchPageManuallyPriceRange_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPageManuallyPriceRange, storeId);
                model.SearchPagePageSizeOptions_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
                model.ProductSearchAutoCompleteEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeId);
                model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeId);
                model.ShowProductImagesInSearchAutoComplete_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeId);
                model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
                model.ProductSearchTermMinimumLength_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchTermMinimumLength, storeId);
                model.ProductsAlsoPurchasedEnabled_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeId);
                model.ProductsAlsoPurchasedNumber_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeId);
                model.NumberOfProductTags_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.NumberOfProductTags, storeId);
                model.ProductsByTagPageSize_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSize, storeId);
                model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeId);
                model.ProductsByTagPageSizeOptions_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeId);
                model.ProductsByTagPriceRangeFiltering_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceRangeFiltering, storeId);
                model.ProductsByTagPriceFrom_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceFrom, storeId);
                model.ProductsByTagPriceTo_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceTo, storeId);
                model.ProductsByTagManuallyPriceRange_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagManuallyPriceRange, storeId);
                model.IncludeShortDescriptionInCompareProducts_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeId);
                model.IncludeFullDescriptionInCompareProducts_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeId);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeId);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeId);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
                model.ShowProductReviewsPerStore_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsPerStore, storeId);
                model.ShowProductReviewsOnAccountPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeId);
                model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, storeId);
                model.ProductReviewsSortByCreatedDateAscending_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsSortByCreatedDateAscending, storeId);
                model.ExportImportProductAttributes_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductAttributes, storeId);
                model.ExportImportProductSpecificationAttributes_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductSpecificationAttributes, storeId);
                model.ExportImportProductCategoryBreadcrumb_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductCategoryBreadcrumb, storeId);
                model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
                model.ExportImportAllowDownloadImages_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
                model.ExportImportSplitProductsFile_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportSplitProductsFile, storeId);
                model.RemoveRequiredProducts_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.RemoveRequiredProducts, storeId);
                model.ExportImportRelatedEntitiesByName_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
                model.ExportImportProductUseLimitedToStores_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductUseLimitedToStores, storeId);
                model.DisplayDatePreOrderAvailability_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
                model.UseAjaxCatalogProductsLoading_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.UseAjaxCatalogProductsLoading, storeId);
                model.EnableManufacturerFiltering_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.EnableManufacturerFiltering, storeId);
                model.EnablePriceRangeFiltering_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.EnablePriceRangeFiltering, storeId);
                model.AttributeValueOutOfStockDisplayType_OverrideForStore = await SettingService.SettingExistsAsync(catalogSettings, x => x.AttributeValueOutOfStockDisplayType, storeId);
            }

            //prepare nested search model
            await PrepareSortOptionSearchModelAsync(model.SortOptionSearchModel);
            await ReviewTypeModelFactory.PrepareReviewTypeSearchModelAsync(model.ReviewTypeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sort option list model
        /// </returns>
        public virtual async Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await SettingService.LoadSettingAsync<CatalogSettings>(storeId);

            //get sort options
            var sortOptions = Enum.GetValues(typeof(ProductSortingEnum)).OfType<ProductSortingEnum>().ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SortOptionListModel().PrepareToGridAsync(searchModel, sortOptions, () =>
            {
                return sortOptions.SelectAwait(async option =>
                {
                    //fill in model values from the entity
                    var sortOptionModel = new SortOptionModel { Id = (int)option };

                    //fill in additional values (not existing in the entity)
                    sortOptionModel.Name = await LocalizationService.GetLocalizedEnumAsync(option);
                    sortOptionModel.IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains((int)option);
                    sortOptionModel.DisplayOrder = catalogSettings
                        .ProductSortingEnumDisplayOrder.TryGetValue((int)option, out var value) ? value : (int)option;

                    return sortOptionModel;
                }).OrderBy(option => option.DisplayOrder);
            });

            return model;
        }

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        /// <param name="model">Reward points settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the reward points settings model
        /// </returns>
        public virtual async Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync(RewardPointsSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var rewardPointsSettings = await SettingService.LoadSettingAsync<RewardPointsSettings>(storeId);

            //fill in model values from the entity
            model ??= rewardPointsSettings.ToSettingsModel<RewardPointsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.ActivatePointsImmediately = model.ActivationDelay <= 0;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.Enabled, storeId);
            model.ExchangeRate_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.ExchangeRate, storeId);
            model.MinimumRewardPointsToUse_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeId);
            model.MaximumRewardPointsToUsePerOrder_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, storeId);
            model.PointsForRegistration_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.RegistrationPointsValidity_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.RegistrationPointsValidity, storeId);
            model.PointsForPurchases_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeId) || await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Points, storeId);
            model.MinOrderTotalToAwardPoints_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, storeId);
            model.PurchasesPointsValidity_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PurchasesPointsValidity, storeId);
            model.ActivationDelay_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.ActivationDelay, storeId);
            model.DisplayHowMuchWillBeEarned_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeId);
            model.PointsForRegistration_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.PageSize_OverrideForStore = await SettingService.SettingExistsAsync(rewardPointsSettings, x => x.PageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <param name="model">Order settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order settings model
        /// </returns>
        public virtual async Task<OrderSettingsModel> PrepareOrderSettingsModelAsync(OrderSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var orderSettings = await SettingService.LoadSettingAsync<OrderSettings>(storeId);

            //fill in model values from the entity
            model ??= orderSettings.ToSettingsModel<OrderSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.OrderIdent = await DataProvider.GetTableIdentAsync<Order>();

            //fill in overridden values
            if (storeId > 0)
            {
                model.IsReOrderAllowed_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.IsReOrderAllowed, storeId);
                model.MinOrderSubtotalAmount_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmount, storeId);
                model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeId);
                model.MinOrderTotalAmount_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.MinOrderTotalAmount, storeId);
                model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeId);
                model.AnonymousCheckoutAllowed_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AnonymousCheckoutAllowed, storeId);
                model.CheckoutDisabled_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.CheckoutDisabled, storeId);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeId);
                model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeId);
                model.OnePageCheckoutEnabled_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutEnabled, storeId);
                model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeId);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeId);
                model.DisableOrderCompletedPage_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.DisableOrderCompletedPage, storeId);
                model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, storeId);
                model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeId);
                model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeId);
                model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeId);
                model.ReturnRequestsEnabled_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsEnabled, storeId);
                model.ReturnRequestsAllowFiles_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsAllowFiles, storeId);
                model.ReturnRequestNumberMask_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestNumberMask, storeId);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeId);
                model.CustomOrderNumberMask_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.CustomOrderNumberMask, storeId);
                model.ExportWithProducts_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.ExportWithProducts, storeId);
                model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.AllowAdminsToBuyCallForPriceProducts, storeId);
                model.DeleteGiftCardUsageHistory_OverrideForStore = await SettingService.SettingExistsAsync(orderSettings, x => x.DeleteGiftCardUsageHistory, storeId);
            }

            //prepare nested search models
            await ReturnRequestModelFactory.PrepareReturnRequestReasonSearchModelAsync(model.ReturnRequestReasonSearchModel);
            await ReturnRequestModelFactory.PrepareReturnRequestActionSearchModelAsync(model.ReturnRequestActionSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <param name="model">Shopping cart settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart settings model
        /// </returns>
        public virtual async Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync(ShoppingCartSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var shoppingCartSettings = await SettingService.LoadSettingAsync<ShoppingCartSettings>(storeId);

            //fill in model values from the entity
            model ??= shoppingCartSettings.ToSettingsModel<ShoppingCartSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayCartAfterAddingProduct_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeId);
            model.DisplayWishlistAfterAddingProduct_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeId);
            model.MaximumShoppingCartItems_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeId);
            model.MaximumWishlistItems_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumWishlistItems, storeId);
            model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeId);
            model.MoveItemsFromWishlistToCart_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeId);
            model.CartsSharedBetweenStores_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeId);
            model.ShowProductImagesOnShoppingCart_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeId);
            model.ShowProductImagesOnWishList_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeId);
            model.ShowDiscountBox_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowDiscountBox, storeId);
            model.ShowGiftCardBox_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowGiftCardBox, storeId);
            model.CrossSellsNumber_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.CrossSellsNumber, storeId);
            model.EmailWishlistEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.EmailWishlistEnabled, storeId);
            model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeId);
            model.MiniShoppingCartEnabled_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeId);
            model.ShowProductImagesInMiniShoppingCart_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeId);
            model.MiniShoppingCartProductNumber_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeId);
            model.AllowCartItemEditing_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowCartItemEditing, storeId);
            model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore = await SettingService.SettingExistsAsync(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, storeId);

            return model;
        }

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <param name="model">Media settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the media settings model
        /// </returns>
        public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var mediaSettings = await SettingService.LoadSettingAsync<MediaSettings>(storeId);

            //fill in model values from the entity
            model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PicturesStoredIntoDatabase = await PictureService.IsStoreInDbAsync();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.AvatarPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
            model.ProductThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSize, storeId);
            model.ProductDetailsPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
            model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
            model.AssociatedProductPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
            model.CategoryThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
            model.ManufacturerThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
            model.VendorThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
            model.CartThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
            model.MiniCartThumbPictureSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
            model.MaximumImageSize_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
            model.MultipleThumbDirectories_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
            model.DefaultImageQuality_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
            model.ImportProductImagesUsingHash_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
            model.DefaultPictureZoomEnabled_OverrideForStore = await SettingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare customer user settings model
        /// </summary>
        /// <param name="model">Customer user settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer user settings model
        /// </returns>
        public virtual async Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync(CustomerUserSettingsModel model = null)
        {
            model ??= new CustomerUserSettingsModel
            {
                ActiveStoreScopeConfiguration = await StoreContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare customer settings model
            model.CustomerSettings = await PrepareCustomerSettingsModelAsync();

            //prepare multi-factor authentication settings model
            model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

            //prepare address settings model
            model.AddressSettings = await PrepareAddressSettingsModelAsync();

            //prepare date time settings model
            model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

            //prepare external authentication settings model
            model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

            //prepare nested search models
            await CustomerAttributeModelFactory.PrepareCustomerAttributeSearchModelAsync(model.CustomerAttributeSearchModel);
            await AddressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <param name="model">Gdpr settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR settings model
        /// </returns>
        public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null)
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var gdprSettings = await SettingService.LoadSettingAsync<GdprSettings>(storeId);

            //fill in model values from the entity
            model ??= gdprSettings.ToSettingsModel<GdprSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //prepare nested search model
            await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.GdprEnabled_OverrideForStore = await SettingService.SettingExistsAsync(gdprSettings, x => x.GdprEnabled, storeId);
            model.LogPrivacyPolicyConsent_OverrideForStore = await SettingService.SettingExistsAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
            model.LogNewsletterConsent_OverrideForStore = await SettingService.SettingExistsAsync(gdprSettings, x => x.LogNewsletterConsent, storeId);
            model.LogUserProfileChanges_OverrideForStore = await SettingService.SettingExistsAsync(gdprSettings, x => x.LogUserProfileChanges, storeId);

            return model;
        }

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent list model
        /// </returns>
        public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get sort options
            var consentList = (await GdprService.GetAllConsentsAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
            {
                return consentList.SelectAwait(async consent =>
                {
                    var gdprConsentModel = consent.ToModel<GdprConsentModel>();

                    var gdprConsent = await GdprService.GetConsentByIdAsync(gdprConsentModel.Id);
                    gdprConsentModel.Message = await LocalizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
                    gdprConsentModel.RequiredMessage = await LocalizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

                    return gdprConsentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare GDPR consent model
        /// </summary>
        /// <param name="model">GDPR consent model</param>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent model
        /// </returns>
        public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
        {
            Func<GdprConsentLocalizedModel, int, Task> localizedModelConfiguration = null;

            //fill in model values from the entity
            if (gdprConsent != null)
            {
                model ??= gdprConsent.ToModel<GdprConsentModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Message = await LocalizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
                    locale.RequiredMessage = await LocalizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
                };
            }

            //set default values for the new model
            if (gdprConsent == null)
                model.DisplayOrder = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await LocalizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <param name="model">General common settings model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the general and common settings model
        /// </returns>
        public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
        {
            model ??= new GeneralCommonSettingsModel
            {
                ActiveStoreScopeConfiguration = await StoreContext.GetActiveStoreScopeConfigurationAsync()
            };

            //prepare store information settings model
            model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

            //prepare Sitemap settings model
            model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

            //prepare Minification settings model
            model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

            //prepare SEO settings model
            model.SeoSettings = await PrepareSeoSettingsModelAsync();

            //prepare security settings model
            model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

            //prepare captcha settings model
            model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

            //prepare PDF settings model
            model.PdfSettings = await PreparePdfSettingsModelAsync();

            //prepare PDF settings model
            model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

            //prepare admin area settings model
            model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

            //prepare display default menu item settings model
            model.DisplayDefaultMenuItemSettings = await PrepareDisplayDefaultMenuItemSettingsModelAsync();

            //prepare display default footer item settings model
            model.DisplayDefaultFooterItemSettings = await PrepareDisplayDefaultFooterItemSettingsModelAsync();

            return model;
        }

        /// <summary>
        /// Prepare product editor settings model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product editor settings model
        /// </returns>
        public virtual async Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModelAsync()
        {
            //load settings for a chosen store scope
            var storeId = await StoreContext.GetActiveStoreScopeConfigurationAsync();
            var productEditorSettings = await SettingService.LoadSettingAsync<ProductEditorSettings>(storeId);

            //fill in model values from the entity
            var model = productEditorSettings.ToSettingsModel<ProductEditorSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting search model
        /// </returns>
        public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare model to add
            await PrepareAddSettingModelAsync(searchModel.AddSetting);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting list model
        /// </returns>
        public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get settings
            var settings = (await SettingService.GetAllSettingsAsync()).AsQueryable();

            //filter settings
            if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
                settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
                settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

            var pagedSettings = settings.ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
            {
                return pagedSettings.SelectAwait(async setting =>
                {
                    //fill in model values from the entity
                    var settingModel = setting.ToModel<SettingModel>();

                    //fill in additional values (not existing in the entity)
                    settingModel.Store = setting.StoreId > 0
                        ? (await StoreService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                        : await LocalizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");

                    return settingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting mode model
        /// </returns>
        public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await GenericAttributeService.GetAttributeAsync<bool>(await WorkContext.GetCurrentCustomerAsync(), modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store scope configuration model
        /// </returns>
        public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
        {
            var model = new StoreScopeConfigurationModel
            {
                Stores = (await StoreService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
                StoreId = await StoreContext.GetActiveStoreScopeConfigurationAsync()
            };

            return model;
        }

        #endregion
    }
}