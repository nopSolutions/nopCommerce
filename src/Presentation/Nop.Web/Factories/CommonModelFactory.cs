using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBlogService _blogService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INewsService _newsService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly SitemapSettings _sitemapSettings;
        private readonly SitemapXmlSettings _sitemapXmlSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CommonModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            IBlogService blogService,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            INopFileProvider fileProvider,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IProductTagService productTagService,
            IShoppingCartService shoppingCartService,
            ISitemapGenerator sitemapGenerator,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            NewsSettings newsSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _customerSettings = customerSettings;
            _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
            _forumSettings = forumSettings;
            _actionContextAccessor = actionContextAccessor;
            _blogService = blogService;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _newsService = newsService;
            _fileProvider = fileProvider;
            _pageHeadBuilder = pageHeadBuilder;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _productTagService = productTagService;
            _shoppingCartService = shoppingCartService;
            _sitemapGenerator = sitemapGenerator;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _themeProvider = themeProvider;
            _topicService = topicService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _localizationSettings = localizationSettings;
            _newsSettings = newsSettings;
            _sitemapSettings = sitemapSettings;
            _sitemapXmlSettings = sitemapXmlSettings;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the number of unread private messages
        /// </summary>
        /// <returns>Number of private messages</returns>
        protected virtual int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.CurrentCustomer;
            if (_forumSettings.AllowPrivateMessages && !customer.IsGuest())
            {
                var privateMessages = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
                    0, customer.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        public virtual LogoModel PrepareLogoModel()
        {
            var model = new LogoModel
            {
                StoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name)
            };

            var cacheKey = string.Format(NopModelCacheDefaults.StoreLogoPath, _storeContext.CurrentStore.Id, _themeContext.WorkingThemeName, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _storeInformationSettings.LogoPictureId;
                if (logoPictureId > 0)
                {
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);
                }
                if (string.IsNullOrEmpty(logo))
                {
                    //use default logo
                    var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty ;
                    var storeLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
                    logo = $"{storeLocation}Themes/{_themeContext.WorkingThemeName}/Content/images/logo.png";
                }
                return logo;
            });

            return model;
        }

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        public virtual LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(string.Format(NopModelCacheDefaults.AvailableLanguagesModelKey, _storeContext.CurrentStore.Id), () =>
            {
                var result = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            return model;
        }

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>Currency selector model</returns>
        public virtual CurrencySelectorModel PrepareCurrencySelectorModel()
        {
            var availableCurrencies = _cacheManager.Get(string.Format(NopModelCacheDefaults.AvailableCurrenciesModelKey, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id), () =>
            {
                var result = _currencyService
                    .GetAllCurrencies(storeId: _storeContext.CurrentStore.Id)
                    .Select(x =>
                    {
                        //currency char
                        var currencySymbol = "";
                        if (!string.IsNullOrEmpty(x.DisplayLocale))
                            currencySymbol = new RegionInfo(x.DisplayLocale).CurrencySymbol;
                        else
                            currencySymbol = x.CurrencyCode;
                        //model
                        var currencyModel = new CurrencyModel
                        {
                            Id = x.Id,
                            Name = _localizationService.GetLocalized(x, y => y.Name),
                            CurrencySymbol = currencySymbol
                        };
                        return currencyModel;
                    })
                    .ToList();
                return result;
            });

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };

            return model;
        }

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>Tax type selector model</returns>
        public virtual TaxTypeSelectorModel PrepareTaxTypeSelectorModel()
        {
            var model = new TaxTypeSelectorModel
            {
                CurrentTaxType = _workContext.TaxDisplayType
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        public virtual HeaderLinksModel PrepareHeaderLinksModel()
        {
            var customer = _workContext.CurrentCustomer;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !_genericAttributeService.GetAttribute<bool>(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, _storeContext.CurrentStore.Id))
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, true, _storeContext.CurrentStore.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerName = customer.IsRegistered() ? _customerService.FormatUsername(customer) : "",
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = customer.IsRegistered() && _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)
            if (customer.HasShoppingCartItems)
            {
                model.ShoppingCartItems = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id)
                    .Sum(item => item.Quantity);

                model.WishlistItems = _shoppingCartService.GetShoppingCart(customer, ShoppingCartType.Wishlist, _storeContext.CurrentStore.Id)
                    .Sum(item => item.Quantity);
            }

            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var customer = _workContext.CurrentCustomer;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedCustomerName = customer.IsRegistered() ? _customerService.FormatUsername(customer) : "",
                IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>Social model</returns>
        public virtual SocialModel PrepareSocialModel()
        {
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                NewsEnabled = _newsSettings.Enabled,
            };

            return model;
        }

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            var topicCacheKey = string.Format(NopModelCacheDefaults.TopicFooterModelKey,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                .Select(t => new FooterModel.FooterTopicModel
                {
                    Id = t.Id,
                    Name = _localizationService.GetLocalized(t, x => x.Title),
                    SeName = _urlRecordService.GetSeName(t),
                    IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                })
                .ToList()
            );

            //model
            var model = new FooterModel
            {
                StoreName = _localizationService.GetLocalized(_storeContext.CurrentStore, x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _sitemapSettings.SitemapEnabled,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                BlogEnabled = _blogSettings.Enabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                NewsEnabled = _newsSettings.Enabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
                AllowCustomersToApplyForVendorAccount = _vendorSettings.AllowCustomersToApplyForVendorAccount,
                AllowCustomersToCheckGiftCardBalance = _customerSettings.AllowCustomersToCheckGiftCardBalance && _captchaSettings.Enabled,
                Topics = cachedTopicModel,
                DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = _displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = _displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = _displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = _displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
                DisplayCustomerInfoFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem,
                DisplayCustomerOrdersFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem,
                DisplayCustomerAddressesFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem,
                DisplayShoppingCartFooterItem = _displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = _displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = _displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        public virtual ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
                model.FullName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
            }
            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

            return model;
        }

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact vendor model</returns>
        public virtual ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
                model.FullName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
            }

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            model.VendorId = vendor.Id;
            model.VendorName = _localizationService.GetLocalized(vendor, x => x.Name);

            return model;
        }

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>Sitemap model</returns>
        public virtual SitemapModel PrepareSitemapModel(SitemapPageModel pageModel)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.SitemapPageModelKey,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);

            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //get URL helper
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                var model = new SitemapModel();

                //prepare common items
                var commonGroupTitle = _localizationService.GetResource("Sitemap.General");

                //home page
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Homepage"),
                    Url = urlHelper.RouteUrl("Homepage")
                });

                //search
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Search"),
                    Url = urlHelper.RouteUrl("ProductSearch")
                });

                //news
                if (_newsSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("News"),
                        Url = urlHelper.RouteUrl("NewsArchive")
                    });
                }

                //blog
                if (_blogSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("Blog"),
                        Url = urlHelper.RouteUrl("Blog")
                    });
                }

                //forums
                if (_forumSettings.ForumsEnabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetResource("Forum.Forums"),
                        Url = urlHelper.RouteUrl("Boards")
                    });
                }

                //contact us
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("ContactUs"),
                    Url = urlHelper.RouteUrl("ContactUs")
                });

                //customer info
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Account.MyAccount"),
                    Url = urlHelper.RouteUrl("CustomerInfo")
                });

                //at the moment topics are in general category too
                if (_sitemapSettings.SitemapIncludeTopics)
                {
                    var topics = _topicService.GetAllTopics(storeId: _storeContext.CurrentStore.Id)
                        .Where(topic => topic.IncludeInSitemap);

                    model.Items.AddRange(topics.Select(topic => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = _localizationService.GetLocalized(topic, x => x.Title),
                        Url = urlHelper.RouteUrl("Topic", new { SeName = _urlRecordService.GetSeName(topic) })
                    }));
                }

                //blog posts
                if (_sitemapSettings.SitemapIncludeBlogPosts && _blogSettings.Enabled)
                {
                    var blogPostsGroupTitle = _localizationService.GetResource("Sitemap.BlogPosts");
                    var blogPosts = _blogService.GetAllBlogPosts(storeId: _storeContext.CurrentStore.Id)
                        .Where(p => p.IncludeInSitemap);

                    model.Items.AddRange(blogPosts.Select(post => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = blogPostsGroupTitle,
                        Name = post.Title,
                        Url = urlHelper.RouteUrl("BlogPost", new { SeName = _urlRecordService.GetSeName(post) })
                    }));
                }

                //news
                if (_sitemapSettings.SitemapIncludeNews && _newsSettings.Enabled)
                {
                    var newsGroupTitle = _localizationService.GetResource("Sitemap.News");
                    var news = _newsService.GetAllNews(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(news.Select(newsItem => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = newsGroupTitle,
                        Name = newsItem.Title,
                        Url = urlHelper.RouteUrl("NewsItem", new { SeName = _urlRecordService.GetSeName(newsItem) })
                    }));
                }

                //categories
                if (_sitemapSettings.SitemapIncludeCategories)
                {
                    var categoriesGroupTitle = _localizationService.GetResource("Sitemap.Categories");
                    var categories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(categories.Select(category => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = categoriesGroupTitle,
                        Name = _localizationService.GetLocalized(category, x => x.Name),
                        Url = urlHelper.RouteUrl("Category", new { SeName = _urlRecordService.GetSeName(category) })
                    }));
                }

                //manufacturers
                if (_sitemapSettings.SitemapIncludeManufacturers)
                {
                    var manufacturersGroupTitle = _localizationService.GetResource("Sitemap.Manufacturers");
                    var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
                    model.Items.AddRange(manufacturers.Select(manufacturer => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = manufacturersGroupTitle,
                        Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                        Url = urlHelper.RouteUrl("Manufacturer", new { SeName = _urlRecordService.GetSeName(manufacturer) })
                    }));
                }

                //products
                if (_sitemapSettings.SitemapIncludeProducts)
                {
                    var productsGroupTitle = _localizationService.GetResource("Sitemap.Products");
                    var products = _productService.SearchProducts(storeId: _storeContext.CurrentStore.Id, visibleIndividuallyOnly: true);
                    model.Items.AddRange(products.Select(product => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productsGroupTitle,
                        Name = _localizationService.GetLocalized(product, x => x.Name),
                        Url = urlHelper.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                    }));
                }

                //product tags
                if (_sitemapSettings.SitemapIncludeProductTags)
                {
                    var productTagsGroupTitle = _localizationService.GetResource("Sitemap.ProductTags");
                    var productTags = _productTagService.GetAllProductTags();
                    model.Items.AddRange(productTags.Select(productTag => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productTagsGroupTitle,
                        Name = _localizationService.GetLocalized(productTag, x => x.Name),
                        Url = urlHelper.RouteUrl("ProductsByTag", new { SeName = _urlRecordService.GetSeName(productTag) })
                    }));
                }

                return model;
            });

            //prepare model with pagination
            pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
            pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

            var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
            var sitemapModel = new SitemapModel { Items = pagedItems };
            sitemapModel.PageModel.LoadPagedList(pagedItems);

            return sitemapModel;
        }

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(int? id)
        {
            var cacheKey = string.Format(NopModelCacheDefaults.SitemapSeoModelKey, id,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(id));
            return siteMap;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        public virtual StoreThemeSelectorModel PrepareStoreThemeSelectorModel()
        {
            var model = new StoreThemeSelectorModel();

            var currentTheme = _themeProvider.GetThemeBySystemName(_themeContext.WorkingThemeName);
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme?.SystemName,
                Title = currentTheme?.FriendlyName
            };

            model.AvailableStoreThemes = _themeProvider.GetThemes().Select(x => new StoreThemeModel
            {
                Name = x.SystemName,
                Title = x.FriendlyName
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        public virtual FaviconAndAppIconsModel PrepareFaviconAndAppIconsModel()
        {
            var model = new FaviconAndAppIconsModel
            {
                HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
            };

            return model;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
            if (_fileProvider.FileExists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart$",
                    "/changecurrency",
                    "/changelanguage",
                    "/changetaxtype",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/customer/avatar",
                    "/customer/activation",
                    "/customer/addresses",
                    "/customer/changepassword",
                    "/customer/checkusernameavailability",
                    "/customer/downloadableproducts",
                    "/customer/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/eucookielawaccept",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/search?",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist",
                };

                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_sitemapXmlSettings.SitemapXmlEnabled)
                {
                    if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    {
                        //URLs are localizable. Append SEO code
                        foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                        {
                            sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _webHelper.GetStoreLocation(), language.UniqueSeoCode);
                            sb.Append(newLine);
                        }
                    }
                    else
                    {
                        //localizable paths (without SEO code)
                        sb.AppendFormat("Sitemap: {0}sitemap.xml", _webHelper.GetStoreLocation());
                        sb.Append(newLine);
                    }
                }
                //host
                sb.AppendFormat("Host: {0}", _webHelper.GetStoreLocation());
                sb.Append(newLine);

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.additions.txt");
                if (_fileProvider.FileExists(robotsAdditionsFile))
                {
                    var robotsFileContent = _fileProvider.ReadAllText(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}