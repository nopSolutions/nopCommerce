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
using Nop.Web.Areas.Admin.Models.Common;
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

        private readonly AppSettings _appSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
        private readonly INopDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IFulltextService _fulltextService;
        private readonly IGdprService _gdprService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReviewTypeModelFactory _reviewTypeModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IThemeProvider _themeProvider;
        private readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SettingModelFactory(AppSettings appSettings,
            CurrencySettings currencySettings,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            ICustomerAttributeModelFactory customerAttributeModelFactory,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IFulltextService fulltextService,
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
            _appSettings = appSettings;
            _currencySettings = currencySettings;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _currencyService = currencyService;
            _customerAttributeModelFactory = customerAttributeModelFactory;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _fulltextService = fulltextService;
            _gdprService = gdprService;
            _localizedModelFactory = localizedModelFactory;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _themeProvider = themeProvider;
            _vendorAttributeModelFactory = vendorAttributeModelFactory;
            _reviewTypeModelFactory = reviewTypeModelFactory;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual async Task PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.CountryEnabled = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CityEnabled = true;
            model.StreetAddressEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;

            //prepare available countries
            await _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            await _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare store theme models
        /// </summary>
        /// <param name="models">List of store theme models</param>
        protected virtual async Task PrepareStoreThemeModels(IList<StoreInformationSettingsModel.ThemeModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var storeInformationSettings = await _settingService.LoadSetting<StoreInformationSettings>(storeId);

            //get available themes
            var availableThemes = await _themeProvider.GetThemes();
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
        /// <returns>Sort option search model</returns>
        protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModel(SortOptionSearchModel searchModel)
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
        /// <returns>GDPR consent search model</returns>
        protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModel(GdprConsentSearchModel searchModel)
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
        /// <returns>Address settings model</returns>
        protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var addressSettings = await _settingService.LoadSetting<AddressSettings>(storeId);

            //fill in model values from the entity
            var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare customer settings model
        /// </summary>
        /// <returns>Customer settings model</returns>
        protected virtual async Task<CustomerSettingsModel> PrepareCustomerSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var customerSettings = await _settingService.LoadSetting<CustomerSettings>(storeId);

            //fill in model values from the entity
            var model = customerSettings.ToSettingsModel<CustomerSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare date time settings model
        /// </summary>
        /// <returns>Date time settings model</returns>
        protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var dateTimeSettings = await _settingService.LoadSetting<DateTimeSettings>(storeId);

            //fill in model values from the entity
            var model = new DateTimeSettingsModel
            {
                AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone
            };

            //fill in additional values (not existing in the entity)
            model.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;

            //prepare available time zones
            await _baseAdminModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            return model;
        }

        /// <summary>
        /// Prepare external authentication settings model
        /// </summary>
        /// <returns>External authentication settings model</returns>
        protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var externalAuthenticationSettings = await _settingService.LoadSetting<ExternalAuthenticationSettings>(storeId);

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
        /// <returns>Store information settings model</returns>
        protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var storeInformationSettings = await _settingService.LoadSetting<StoreInformationSettings>(storeId);
            var commonSettings = await _settingService.LoadSetting<CommonSettings>(storeId);

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
            await PrepareStoreThemeModels(model.AvailableStoreThemes);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.StoreClosed_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeId);
            model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
            model.AllowCustomerToSelectTheme_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.LogoPictureId, storeId);
            model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
            model.FacebookLink_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeId);
            model.TwitterLink_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeId);
            model.YoutubeLink_OverrideForStore = await _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeId);
            model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
            model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
            model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExists(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

            return model;
        }

        /// <summary>
        /// Prepare Sitemap settings model
        /// </summary>
        /// <returns>Sitemap settings model</returns>
        protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var sitemapSettings = await _settingService.LoadSetting<SitemapSettings>(storeId);

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
            model.SitemapEnabled_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapEnabled, storeId);
            model.SitemapPageSize_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapPageSize, storeId);
            model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
            model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
            model.SitemapIncludeProducts_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
            model.SitemapIncludeProductTags_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
            model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
            model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeNews, storeId);
            model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExists(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

            return model;
        }

        /// <summary>
        /// Prepare minification settings model
        /// </summary>
        /// <returns>Minification settings model</returns>
        protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var minificationSettings = await _settingService.LoadSetting<CommonSettings>(storeId);

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
            model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExists(minificationSettings, x => x.EnableHtmlMinification, storeId);
            model.EnableJsBundling_OverrideForStore = await _settingService.SettingExists(minificationSettings, x => x.EnableJsBundling, storeId);
            model.EnableCssBundling_OverrideForStore = await _settingService.SettingExists(minificationSettings, x => x.EnableCssBundling, storeId);
            model.UseResponseCompression_OverrideForStore = await _settingService.SettingExists(minificationSettings, x => x.UseResponseCompression, storeId);

            return model;
        }

        /// <summary>
        /// Prepare SEO settings model
        /// </summary>
        /// <returns>SEO settings model</returns>
        protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var seoSettings = await _settingService.LoadSetting<SeoSettings>(storeId);

            //fill in model values from the entity
            var model = new SeoSettingsModel
            {
                PageTitleSeparator = seoSettings.PageTitleSeparator,
                PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
                PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList(),
                DefaultTitle = seoSettings.DefaultTitle,
                DefaultMetaKeywords = seoSettings.DefaultMetaKeywords,
                DefaultMetaDescription = seoSettings.DefaultMetaDescription,
                GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
                ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
                CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
                WwwRequirement = (int)seoSettings.WwwRequirement,
                WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList(),

                TwitterMetaTags = seoSettings.TwitterMetaTags,
                OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
                CustomHeadTags = seoSettings.CustomHeadTags,
                MicrodataEnabled = seoSettings.MicrodataEnabled
            };

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator, storeId);
            model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
            model.DefaultTitle_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.DefaultTitle, storeId);
            model.DefaultMetaKeywords_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords, storeId);
            model.DefaultMetaDescription_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription, storeId);
            model.GenerateProductMetaDescription_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.GenerateProductMetaDescription, storeId);
            model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars, storeId);
            model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
            model.WwwRequirement_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.WwwRequirement, storeId);
            model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags, storeId);
            model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags, storeId);
            model.CustomHeadTags_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.CustomHeadTags, storeId);
            model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExists(seoSettings, x => x.MicrodataEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare security settings model
        /// </summary>
        /// <returns>Security settings model</returns>
        protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var securitySettings = await _settingService.LoadSetting<SecuritySettings>(storeId);

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
        /// <returns>Captcha settings model</returns>
        protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var captchaSettings = await _settingService.LoadSetting<CaptchaSettings>(storeId);

            //fill in model values from the entity
            var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

            model.CaptchaTypeValues = captchaSettings.CaptchaType.ToSelectList();

            if (storeId <= 0)
                return model;

            model.Enabled_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.Enabled, storeId);
            model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnLoginPage, storeId);
            model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
            model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnContactUsPage, storeId);
            model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
            model.ShowOnEmailProductToFriendPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
            model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
            model.ShowOnNewsCommentPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
            model.ShowOnProductReviewPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
            model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
            model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
            model.ShowOnForum_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ShowOnForum, storeId);
            model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
            model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
            model.CaptchaType_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.CaptchaType, storeId);
            model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExists(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

            return model;
        }

        /// <summary>
        /// Prepare PDF settings model
        /// </summary>
        /// <returns>PDF settings model</returns>
        protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var pdfSettings = await _settingService.LoadSetting<PdfSettings>(storeId);

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
            model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExists(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
            model.LogoPictureId_OverrideForStore = await _settingService.SettingExists(pdfSettings, x => x.LogoPictureId, storeId);
            model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExists(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
            model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
            model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

            return model;
        }

        /// <summary>
        /// Prepare localization settings model
        /// </summary>
        /// <returns>Localization settings model</returns>
        protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var localizationSettings = await _settingService.LoadSetting<LocalizationSettings>(storeId);

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
        /// Prepare full-text settings model
        /// </summary>
        /// <returns>Full-text settings model</returns>
        protected virtual async Task<FullTextSettingsModel> PrepareFullTextSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var commonSettings = await _settingService.LoadSetting<CommonSettings>(storeId);

            //fill in model values from the entity
            var model = new FullTextSettingsModel
            {
                Enabled = commonSettings.UseFullTextSearch,
                SearchMode = (int)commonSettings.FullTextMode
            };

            //fill in additional values (not existing in the entity)
            model.Supported = await _fulltextService.IsFullTextSupported();
            model.SearchModeValues = commonSettings.FullTextMode.ToSelectList();

            return model;
        }

        /// <summary>
        /// Prepare admin area settings model
        /// </summary>
        /// <returns>Admin area settings model</returns>
        protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var adminAreaSettings = await _settingService.LoadSetting<AdminAreaSettings>(storeId);

            //fill in model values from the entity
            var model = new AdminAreaSettingsModel
            {
                UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates
            };

            //fill in overridden values
            if (storeId > 0)
            {
                model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExists(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
            }

            return model;
        }

        /// <summary>
        /// Prepare display default menu item settings model
        /// </summary>
        /// <returns>Display default menu item settings model</returns>
        protected virtual async Task<DisplayDefaultMenuItemSettingsModel> PrepareDisplayDefaultMenuItemSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var displayDefaultMenuItemSettings = await _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeId);

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
            model.DisplayHomepageMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayHomepageMenuItem, storeId);
            model.DisplayNewProductsMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, storeId);
            model.DisplayProductSearchMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, storeId);
            model.DisplayCustomerInfoMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, storeId);
            model.DisplayBlogMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeId);
            model.DisplayForumsMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeId);
            model.DisplayContactUsMenuItem_OverrideForStore = await _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare display default footer item settings model
        /// </summary>
        /// <returns>Display default footer item settings model</returns>
        protected virtual async Task<DisplayDefaultFooterItemSettingsModel> PrepareDisplayDefaultFooterItemSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var displayDefaultFooterItemSettings = await _settingService.LoadSetting<DisplayDefaultFooterItemSettings>(storeId);

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
            model.DisplaySitemapFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplaySitemapFooterItem, storeId);
            model.DisplayContactUsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayContactUsFooterItem, storeId);
            model.DisplayProductSearchFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayProductSearchFooterItem, storeId);
            model.DisplayNewsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayNewsFooterItem, storeId);
            model.DisplayBlogFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayBlogFooterItem, storeId);
            model.DisplayForumsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayForumsFooterItem, storeId);
            model.DisplayRecentlyViewedProductsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayRecentlyViewedProductsFooterItem, storeId);
            model.DisplayCompareProductsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayCompareProductsFooterItem, storeId);
            model.DisplayNewProductsFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayNewProductsFooterItem, storeId);
            model.DisplayCustomerInfoFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayCustomerInfoFooterItem, storeId);
            model.DisplayCustomerOrdersFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayCustomerOrdersFooterItem, storeId);
            model.DisplayCustomerAddressesFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayCustomerAddressesFooterItem, storeId);
            model.DisplayShoppingCartFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayShoppingCartFooterItem, storeId);
            model.DisplayWishlistFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayWishlistFooterItem, storeId);
            model.DisplayApplyVendorAccountFooterItem_OverrideForStore = await _settingService.SettingExists(displayDefaultFooterItemSettings, x => x.DisplayApplyVendorAccountFooterItem, storeId);

            return model;
        }

        /// <summary>
        /// Prepare setting model to add
        /// </summary>
        /// <param name="model">Setting model to add</param>
        protected virtual async Task PrepareAddSettingModel(SettingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStores(model.AvailableStores);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <returns>App settings model</returns>
        public virtual AppSettingsModel PrepareAppSettingsModel()
        {
            var model = new AppSettingsModel
            {
                CacheConfigModel = _appSettings.CacheConfig.ToConfigModel<CacheConfigModel>(),
                HostingConfigModel = _appSettings.HostingConfig.ToConfigModel<HostingConfigModel>(),
                RedisConfigModel = _appSettings.RedisConfig.ToConfigModel<RedisConfigModel>(),
                AzureBlobConfigModel = _appSettings.AzureBlobConfig.ToConfigModel<AzureBlobConfigModel>(),
                InstallationConfigModel = _appSettings.InstallationConfig.ToConfigModel<InstallationConfigModel>(),
                PluginConfigModel = _appSettings.PluginConfig.ToConfigModel<PluginConfigModel>(),
                CommonConfigModel = _appSettings.CommonConfig.ToConfigModel<CommonConfigModel>()
            };

            return model;
        }

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <returns>Blog settings model</returns>
        public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var blogSettings = await _settingService.LoadSetting<BlogSettings>(storeId);

            //fill in model values from the entity
            var model = blogSettings.ToSettingsModel<BlogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.Enabled, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.PostsPageSize, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewBlogComments_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
            model.NumberOfTags_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.NumberOfTags, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.ShowHeaderRssUrl, storeId);
            model.BlogCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExists(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <returns>Vendor settings model</returns>
        public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var vendorSettings = await _settingService.LoadSetting<VendorSettings>(storeId);

            //fill in model values from the entity
            var model = vendorSettings.ToSettingsModel<VendorSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.VendorsBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
                model.ShowVendorOnProductDetailsPage_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeId);
                model.ShowVendorOnOrderDetailsPage_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
                model.AllowCustomersToContactVendors_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.AllowCustomersToContactVendors, storeId);
                model.AllowCustomersToApplyForVendorAccount_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeId);
                model.TermsOfServiceEnabled_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
                model.AllowSearchByVendor_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.AllowSearchByVendor, storeId);
                model.AllowVendorsToEditInfo_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
                model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
                model.MaximumProductNumber_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.MaximumProductNumber, storeId);
                model.AllowVendorsToImportProducts_OverrideForStore = await _settingService.SettingExists(vendorSettings, x => x.AllowVendorsToImportProducts, storeId);
            }

            //prepare nested search model
            await _vendorAttributeModelFactory.PrepareVendorAttributeSearchModel(model.VendorAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <returns>Forum settings model</returns>
        public virtual async Task<ForumSettingsModel> PrepareForumSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var forumSettings = await _settingService.LoadSetting<ForumSettings>(storeId);

            //fill in model values from the entity
            var model = forumSettings.ToSettingsModel<ForumSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.ForumsEnabled_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ForumsEnabled, storeId);
            model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
            model.ShowCustomersPostCount_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ShowCustomersPostCount, storeId);
            model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
            model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
            model.AllowCustomersToEditPosts_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowCustomersToEditPosts, storeId);
            model.AllowCustomersToDeletePosts_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowCustomersToDeletePosts, storeId);
            model.AllowPostVoting_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowPostVoting, storeId);
            model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.MaxVotesPerDay, storeId);
            model.AllowCustomersToManageSubscriptions_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeId);
            model.TopicsPageSize_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.TopicsPageSize, storeId);
            model.PostsPageSize_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.PostsPageSize, storeId);
            model.ForumEditor_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ForumEditor, storeId);
            model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.SignaturesEnabled, storeId);
            model.AllowPrivateMessages_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.AllowPrivateMessages, storeId);
            model.ShowAlertForPM_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ShowAlertForPM, storeId);
            model.NotifyAboutPrivateMessages_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
            model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
            model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
            model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ForumFeedsEnabled, storeId);
            model.ForumFeedCount_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ForumFeedCount, storeId);
            model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.SearchResultsPageSize, storeId);
            model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExists(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <returns>News settings model</returns>
        public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var newsSettings = await _settingService.LoadSetting<NewsSettings>(storeId);

            //fill in model values from the entity
            var model = newsSettings.ToSettingsModel<NewsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.Enabled, storeId);
            model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
            model.NotifyAboutNewNewsComments_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
            model.ShowNewsOnMainPage_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.ShowNewsOnMainPage, storeId);
            model.MainPageNewsCount_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.MainPageNewsCount, storeId);
            model.NewsArchivePageSize_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.NewsArchivePageSize, storeId);
            model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.ShowHeaderRssUrl, storeId);
            model.NewsCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExists(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);

            return model;
        }

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <returns>Shipping settings model</returns>
        public virtual async Task<ShippingSettingsModel> PrepareShippingSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var shippingSettings = await _settingService.LoadSetting<ShippingSettings>(storeId);

            //fill in model values from the entity
            var model = shippingSettings.ToSettingsModel<ShippingSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;

            //fill in overridden values
            if (storeId > 0)
            {
                model.ShipToSameAddress_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.ShipToSameAddress, storeId);
                model.AllowPickupInStore_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.AllowPickupInStore, storeId);
                model.DisplayPickupPointsOnMap_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.DisplayPickupPointsOnMap, storeId);
                model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, storeId);
                model.GoogleMapsApiKey_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.GoogleMapsApiKey, storeId);
                model.UseWarehouseLocation_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.UseWarehouseLocation, storeId);
                model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeId);
                model.FreeShippingOverXEnabled_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXEnabled, storeId);
                model.FreeShippingOverXValue_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXValue, storeId);
                model.FreeShippingOverXIncludingTax_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeId);
                model.EstimateShippingCartPageEnabled_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.EstimateShippingCartPageEnabled, storeId);
                model.EstimateShippingProductPageEnabled_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.EstimateShippingProductPageEnabled, storeId);
                model.DisplayShipmentEventsToCustomers_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeId);
                model.DisplayShipmentEventsToStoreOwner_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeId);
                model.HideShippingTotal_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.HideShippingTotal, storeId);
                model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeId);
                model.ConsiderAssociatedProductsDimensions_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.ConsiderAssociatedProductsDimensions, storeId);
                model.ShippingOriginAddress_OverrideForStore = await _settingService.SettingExists(shippingSettings, x => x.ShippingOriginAddressId, storeId);
            }

            //prepare shipping origin address
            var originAddress = await _addressService.GetAddressById(shippingSettings.ShippingOriginAddressId);
            if (originAddress != null)
                model.ShippingOriginAddress = originAddress.ToModel(model.ShippingOriginAddress);
            await PrepareAddressModel(model.ShippingOriginAddress, originAddress);

            return model;
        }

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <returns>Tax settings model</returns>
        public virtual async Task<TaxSettingsModel> PrepareTaxSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var taxSettings = await _settingService.LoadSetting<TaxSettings>(storeId);

            //fill in model values from the entity
            var model = taxSettings.ToSettingsModel<TaxSettingsModel>();
            model.TaxBasedOnValues = taxSettings.TaxBasedOn.ToSelectList();
            model.TaxDisplayTypeValues = taxSettings.TaxDisplayType.ToSelectList();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.PricesIncludeTax_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.PricesIncludeTax, storeId);
                model.AllowCustomersToSelectTaxDisplayType_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeId);
                model.TaxDisplayType_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.TaxDisplayType, storeId);
                model.DisplayTaxSuffix_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.DisplayTaxSuffix, storeId);
                model.DisplayTaxRates_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.DisplayTaxRates, storeId);
                model.HideZeroTax_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.HideZeroTax, storeId);
                model.HideTaxInOrderSummary_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.HideTaxInOrderSummary, storeId);
                model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeId);
                model.DefaultTaxCategoryId_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.DefaultTaxCategoryId, storeId);
                model.TaxBasedOn_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.TaxBasedOn, storeId);
                model.TaxBasedOnPickupPointAddress_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeId);
                model.DefaultTaxAddress_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.DefaultTaxAddressId, storeId);
                model.ShippingIsTaxable_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.ShippingIsTaxable, storeId);
                model.ShippingPriceIncludesTax_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.ShippingPriceIncludesTax, storeId);
                model.ShippingTaxClassId_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.ShippingTaxClassId, storeId);
                model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeId);
                model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeId);
                model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeId);
                model.EuVatEnabled_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatEnabled, storeId);
                model.EuVatShopCountryId_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatShopCountryId, storeId);
                model.EuVatAllowVatExemption_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatAllowVatExemption, storeId);
                model.EuVatUseWebService_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatUseWebService, storeId);
                model.EuVatAssumeValid_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatAssumeValid, storeId);
                model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = await _settingService.SettingExists(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeId);
            }

            //prepare available tax categories
            await _baseAdminModelFactory.PrepareTaxCategories(model.TaxCategories);

            //prepare available EU VAT countries
            await _baseAdminModelFactory.PrepareCountries(model.EuVatShopCountries);

            //prepare default tax address
            var defaultAddress = await _addressService.GetAddressById(taxSettings.DefaultTaxAddressId);
            if (defaultAddress != null)
                model.DefaultTaxAddress = defaultAddress.ToModel(model.DefaultTaxAddress);
            await PrepareAddressModel(model.DefaultTaxAddress, defaultAddress);

            return model;
        }

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <returns>Catalog settings model</returns>
        public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var catalogSettings = await _settingService.LoadSetting<CatalogSettings>(storeId);

            //fill in model values from the entity
            var model = catalogSettings.ToSettingsModel<CatalogSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResource("Admin.Catalog.ViewMode.Grid"),
                Value = "grid"
            });
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResource("Admin.Catalog.ViewMode.List"),
                Value = "list"
            });

            //fill in overridden values
            if (storeId > 0)
            {
                model.AllowViewUnpublishedProductPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeId);
                model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeId);
                model.ShowSkuOnProductDetailsPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeId);
                model.ShowSkuOnCatalogPages_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
                model.ShowManufacturerPartNumber_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
                model.ShowGtin_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowGtin, storeId);
                model.ShowFreeShippingNotification_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
                model.AllowProductSorting_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.AllowProductSorting, storeId);
                model.AllowProductViewModeChanging_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.AllowProductViewModeChanging, storeId);
                model.DefaultViewMode_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DefaultViewMode, storeId);
                model.ShowProductsFromSubcategories_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowProductsFromSubcategories, storeId);
                model.ShowCategoryProductNumber_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumber, storeId);
                model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeId);
                model.CategoryBreadcrumbEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
                model.ShowShareButton_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowShareButton, storeId);
                model.PageShareCode_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.PageShareCode, storeId);
                model.ProductReviewsMustBeApproved_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductReviewsMustBeApproved, storeId);
                model.OneReviewPerProductFromCustomer_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.OneReviewPerProductFromCustomer, storeId);
                model.AllowAnonymousUsersToReviewProduct_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeId);
                model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeId);
                model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeId);
                model.NotifyCustomerAboutProductReviewReply_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NotifyCustomerAboutProductReviewReply, storeId);
                model.EmailAFriendEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.EmailAFriendEnabled, storeId);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
                model.RecentlyViewedProductsNumber_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsNumber, storeId);
                model.RecentlyViewedProductsEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeId);
                model.NewProductsNumber_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NewProductsNumber, storeId);
                model.NewProductsEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NewProductsEnabled, storeId);
                model.CompareProductsEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.CompareProductsEnabled, storeId);
                model.ShowBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
                model.SearchPageProductsPerPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.SearchPageProductsPerPage, storeId);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeId);
                model.SearchPagePageSizeOptions_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
                model.ProductSearchAutoCompleteEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeId);
                model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeId);
                model.ShowProductImagesInSearchAutoComplete_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeId);
                model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
                model.ProductSearchTermMinimumLength_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductSearchTermMinimumLength, storeId);
                model.ProductsAlsoPurchasedEnabled_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeId);
                model.ProductsAlsoPurchasedNumber_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeId);
                model.NumberOfProductTags_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.NumberOfProductTags, storeId);
                model.ProductsByTagPageSize_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSize, storeId);
                model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeId);
                model.ProductsByTagPageSizeOptions_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeId);
                model.IncludeShortDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeId);
                model.IncludeFullDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeId);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeId);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeId);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
                model.ShowProductReviewsPerStore_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsPerStore, storeId);
                model.ShowProductReviewsOnAccountPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeId);
                model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, storeId);
                model.ProductReviewsSortByCreatedDateAscending_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ProductReviewsSortByCreatedDateAscending, storeId);
                model.ExportImportProductAttributes_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportProductAttributes, storeId);
                model.ExportImportProductSpecificationAttributes_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportProductSpecificationAttributes, storeId);
                model.ExportImportProductCategoryBreadcrumb_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportProductCategoryBreadcrumb, storeId);
                model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
                model.ExportImportAllowDownloadImages_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
                model.ExportImportSplitProductsFile_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportSplitProductsFile, storeId);
                model.RemoveRequiredProducts_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.RemoveRequiredProducts, storeId);
                model.ExportImportRelatedEntitiesByName_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
                model.ExportImportProductUseLimitedToStores_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.ExportImportProductUseLimitedToStores, storeId);
                model.DisplayDatePreOrderAvailability_OverrideForStore = await _settingService.SettingExists(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
            }

            //prepare nested search model
            await PrepareSortOptionSearchModel(model.SortOptionSearchModel);
            await _reviewTypeModelFactory.PrepareReviewTypeSearchModel(model.ReviewTypeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option list model</returns>
        public virtual async Task<SortOptionListModel> PrepareSortOptionListModel(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var catalogSettings = await _settingService.LoadSetting<CatalogSettings>(storeId);

            //get sort options
            var sortOptions = Enum.GetValues(typeof(ProductSortingEnum)).OfType<ProductSortingEnum>().ToList().ToPagedList(searchModel);

            //prepare list model
            var model = new SortOptionListModel().PrepareToGrid(searchModel, sortOptions, () =>
            {
                return sortOptions.Select(option =>
                {
                    //fill in model values from the entity
                    var sortOptionModel = new SortOptionModel
                    {
                        Id = (int)option
                    };

                    //fill in additional values (not existing in the entity)
                    sortOptionModel.Name = _localizationService.GetLocalizedEnum(option).Result;
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
        /// <returns>Reward points settings model</returns>
        public virtual async Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var rewardPointsSettings = await _settingService.LoadSetting<RewardPointsSettings>(storeId);

            //fill in model values from the entity
            var model = rewardPointsSettings.ToSettingsModel<RewardPointsSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.ActivatePointsImmediately = model.ActivationDelay <= 0;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.Enabled_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.Enabled, storeId);
            model.ExchangeRate_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.ExchangeRate, storeId);
            model.MinimumRewardPointsToUse_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeId);
            model.MaximumRewardPointsToUsePerOrder_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, storeId);
            model.PointsForRegistration_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.RegistrationPointsValidity_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.RegistrationPointsValidity, storeId);
            model.PointsForPurchases_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeId) || await _settingService.SettingExists(rewardPointsSettings, x => x.PointsForPurchases_Points, storeId);
            model.MinOrderTotalToAwardPoints_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, storeId);
            model.PurchasesPointsValidity_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.PurchasesPointsValidity, storeId);
            model.ActivationDelay_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.ActivationDelay, storeId);
            model.DisplayHowMuchWillBeEarned_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeId);
            model.PointsForRegistration_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.PointsForRegistration, storeId);
            model.PageSize_OverrideForStore = await _settingService.SettingExists(rewardPointsSettings, x => x.PageSize, storeId);

            return model;
        }

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <returns>Order settings model</returns>
        public virtual async Task<OrderSettingsModel> PrepareOrderSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var orderSettings = await _settingService.LoadSetting<OrderSettings>(storeId);

            //fill in model values from the entity
            var model = orderSettings.ToSettingsModel<OrderSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.OrderIdent = _dataProvider.GetTableIdent<Order>();

            //fill in overridden values
            if (storeId > 0)
            {
                model.IsReOrderAllowed_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.IsReOrderAllowed, storeId);
                model.MinOrderSubtotalAmount_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.MinOrderSubtotalAmount, storeId);
                model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeId);
                model.MinOrderTotalAmount_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.MinOrderTotalAmount, storeId);
                model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeId);
                model.AnonymousCheckoutAllowed_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AnonymousCheckoutAllowed, storeId);
                model.CheckoutDisabled_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.CheckoutDisabled, storeId);
                model.TermsOfServiceOnShoppingCartPage_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeId);
                model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeId);
                model.OnePageCheckoutEnabled_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.OnePageCheckoutEnabled, storeId);
                model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeId);
                model.DisableBillingAddressCheckoutStep_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeId);
                model.DisableOrderCompletedPage_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.DisableOrderCompletedPage, storeId);
                model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, storeId);
                model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeId);
                model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeId);
                model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeId);
                model.ReturnRequestsEnabled_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.ReturnRequestsEnabled, storeId);
                model.ReturnRequestsAllowFiles_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.ReturnRequestsAllowFiles, storeId);
                model.ReturnRequestNumberMask_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.ReturnRequestNumberMask, storeId);
                model.NumberOfDaysReturnRequestAvailable_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeId);
                model.CustomOrderNumberMask_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.CustomOrderNumberMask, storeId);
                model.ExportWithProducts_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.ExportWithProducts, storeId);
                model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.AllowAdminsToBuyCallForPriceProducts, storeId);
                model.DeleteGiftCardUsageHistory_OverrideForStore = await _settingService.SettingExists(orderSettings, x => x.DeleteGiftCardUsageHistory, storeId);
            }

            //prepare nested search models
            await _returnRequestModelFactory.PrepareReturnRequestReasonSearchModel(model.ReturnRequestReasonSearchModel);
            await _returnRequestModelFactory.PrepareReturnRequestActionSearchModel(model.ReturnRequestActionSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <returns>Shopping cart settings model</returns>
        public virtual async Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var shoppingCartSettings = await _settingService.LoadSetting<ShoppingCartSettings>(storeId);

            //fill in model values from the entity
            var model = shoppingCartSettings.ToSettingsModel<ShoppingCartSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.DisplayCartAfterAddingProduct_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeId);
            model.DisplayWishlistAfterAddingProduct_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeId);
            model.MaximumShoppingCartItems_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeId);
            model.MaximumWishlistItems_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.MaximumWishlistItems, storeId);
            model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeId);
            model.MoveItemsFromWishlistToCart_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeId);
            model.CartsSharedBetweenStores_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeId);
            model.ShowProductImagesOnShoppingCart_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeId);
            model.ShowProductImagesOnWishList_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeId);
            model.ShowDiscountBox_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.ShowDiscountBox, storeId);
            model.ShowGiftCardBox_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.ShowGiftCardBox, storeId);
            model.CrossSellsNumber_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.CrossSellsNumber, storeId);
            model.EmailWishlistEnabled_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.EmailWishlistEnabled, storeId);
            model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeId);
            model.MiniShoppingCartEnabled_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeId);
            model.ShowProductImagesInMiniShoppingCart_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeId);
            model.MiniShoppingCartProductNumber_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeId);
            model.AllowCartItemEditing_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.AllowCartItemEditing, storeId);
            model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore = await _settingService.SettingExists(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, storeId);

            return model;
        }

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <returns>Media settings model</returns>
        public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var mediaSettings = await _settingService.LoadSetting<MediaSettings>(storeId);

            //fill in model values from the entity
            var model = mediaSettings.ToSettingsModel<MediaSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, storeId);
            model.ProductThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSize, storeId);
            model.ProductDetailsPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
            model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
            model.AssociatedProductPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
            model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
            model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
            model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.VendorThumbPictureSize, storeId);
            model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.CartThumbPictureSize, storeId);
            model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
            model.MaximumImageSize_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, storeId);
            model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.MultipleThumbDirectories, storeId);
            model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, storeId);
            model.ImportProductImagesUsingHash_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
            model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExists(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);

            return model;
        }

        /// <summary>
        /// Prepare customer user settings model
        /// </summary>
        /// <returns>Customer user settings model</returns>
        public virtual async Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModel()
        {
            var model = new CustomerUserSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfiguration()
            };

            //prepare customer settings model
            model.CustomerSettings = await PrepareCustomerSettingsModel();

            //prepare address settings model
            model.AddressSettings = await PrepareAddressSettingsModel();

            //prepare date time settings model
            model.DateTimeSettings = await PrepareDateTimeSettingsModel();

            //prepare external authentication settings model
            model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModel();

            //prepare nested search models
            await _customerAttributeModelFactory.PrepareCustomerAttributeSearchModel(model.CustomerAttributeSearchModel);
            await _addressAttributeModelFactory.PrepareAddressAttributeSearchModel(model.AddressAttributeSearchModel);

            return model;
        }

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <returns>GDPR settings model</returns>
        public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var gdprSettings = await _settingService.LoadSetting<GdprSettings>(storeId);

            //fill in model values from the entity
            var model = gdprSettings.ToSettingsModel<GdprSettingsModel>();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //prepare nested search model
            await PrepareGdprConsentSearchModel(model.GdprConsentSearchModel);

            if (storeId <= 0)
                return model;

            //fill in overridden values
            model.GdprEnabled_OverrideForStore = await _settingService.SettingExists(gdprSettings, x => x.GdprEnabled, storeId);
            model.LogPrivacyPolicyConsent_OverrideForStore = await _settingService.SettingExists(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
            model.LogNewsletterConsent_OverrideForStore = await _settingService.SettingExists(gdprSettings, x => x.LogNewsletterConsent, storeId);
            model.LogUserProfileChanges_OverrideForStore = await _settingService.SettingExists(gdprSettings, x => x.LogUserProfileChanges, storeId);

            return model;
        }

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>GDPR consent list model</returns>
        public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModel(GdprConsentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get sort options
            var consentList = (await _gdprService.GetAllConsents()).ToPagedList(searchModel);

            //prepare list model
            var model = new GdprConsentListModel().PrepareToGrid(searchModel, consentList, () =>
            {
                return consentList.Select(consent =>
                {
                    var gdprConsentModel = consent.ToModel<GdprConsentModel>();
                    var gdprConsent = _gdprService.GetConsentById(gdprConsentModel.Id).Result;
                    gdprConsentModel.Message = _localizationService.GetLocalized(gdprConsent, entity => entity.Message).Result;
                    gdprConsentModel.RequiredMessage = _localizationService.GetLocalized(gdprConsent, entity => entity.RequiredMessage).Result;

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
        /// <returns>GDPR consent model</returns>
        public virtual async Task<GdprConsentModel> PrepareGdprConsentModel(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
        {
            Action<GdprConsentLocalizedModel, int> localizedModelConfiguration = null;

            //fill in model values from the entity
            if (gdprConsent != null)
            {
                model ??= gdprConsent.ToModel<GdprConsentModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Message = await _localizationService.GetLocalized(gdprConsent, entity => entity.Message, languageId, false, false);
                    locale.RequiredMessage = await _localizationService.GetLocalized(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
                };
            }

            //set default values for the new model
            if (gdprConsent == null)
                model.DisplayOrder = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <returns>General and common settings model</returns>
        public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModel()
        {
            var model = new GeneralCommonSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfiguration()
            };

            //prepare store information settings model
            model.StoreInformationSettings = await PrepareStoreInformationSettingsModel();

            //prepare Sitemap settings model
            model.SitemapSettings = await PrepareSitemapSettingsModel();

            //prepare Minification settings model
            model.MinificationSettings = await PrepareMinificationSettingsModel();

            //prepare SEO settings model
            model.SeoSettings = await PrepareSeoSettingsModel();

            //prepare security settings model
            model.SecuritySettings = await PrepareSecuritySettingsModel();

            //prepare captcha settings model
            model.CaptchaSettings = await PrepareCaptchaSettingsModel();

            //prepare PDF settings model
            model.PdfSettings = await PreparePdfSettingsModel();

            //prepare PDF settings model
            model.LocalizationSettings = await PrepareLocalizationSettingsModel();

            //prepare full-text settings model
            model.FullTextSettings = await PrepareFullTextSettingsModel();

            //prepare admin area settings model
            model.AdminAreaSettings = await PrepareAdminAreaSettingsModel();

            //prepare display default menu item settings model
            model.DisplayDefaultMenuItemSettings = await PrepareDisplayDefaultMenuItemSettingsModel();

            //prepare display default footer item settings model
            model.DisplayDefaultFooterItemSettings = await PrepareDisplayDefaultFooterItemSettingsModel();

            return model;
        }

        /// <summary>
        /// Prepare product editor settings model
        /// </summary>
        /// <returns>Product editor settings model</returns>
        public virtual async Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModel()
        {
            //load settings for a chosen store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfiguration();
            var productEditorSettings = await _settingService.LoadSetting<ProductEditorSettings>(storeId);

            //fill in model values from the entity
            var model = productEditorSettings.ToSettingsModel<ProductEditorSettingsModel>();

            return model;
        }

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting search model</returns>
        public virtual async Task<SettingSearchModel> PrepareSettingSearchModel(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare model to add
            await PrepareAddSettingModel(searchModel.AddSetting);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>Setting list model</returns>
        public virtual async Task<SettingListModel> PrepareSettingListModel(SettingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get settings
            var settings = (await _settingService.GetAllSettings()).AsQueryable();

            //filter settings
            if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
                settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
                settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

            var pagedSettings = settings.ToList().ToPagedList(searchModel);

            //prepare list model
            var model = new SettingListModel().PrepareToGrid(searchModel, pagedSettings, () =>
            {
                return pagedSettings.Select(setting =>
                {
                    //fill in model values from the entity
                    var settingModel = setting.ToModel<SettingModel>();

                    //fill in additional values (not existing in the entity)
                    settingModel.Store = setting.StoreId > 0
                        ? _storeService.GetStoreById(setting.StoreId).Result?.Name ?? "Deleted"
                        : _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores").Result;

                    return settingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        public virtual async Task<SettingModeModel> PrepareSettingModeModel(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>Store scope configuration model</returns>
        public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModel()
        {
            var model = new StoreScopeConfigurationModel
            {
                Stores = (await _storeService.GetAllStores()).Select(store => store.ToModel<StoreModel>()).ToList(),
                StoreId = await _storeContext.GetActiveStoreScopeConfiguration()
            };

            return model;
        }

        #endregion
    }
}