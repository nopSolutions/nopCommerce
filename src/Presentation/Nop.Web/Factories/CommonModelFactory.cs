using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected BlogSettings BlogSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CommonSettings CommonSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected DisplayDefaultFooterItemSettings DisplayDefaultFooterItemSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected IActionContextAccessor ActionContextAccessor { get; }
        protected IBlogService BlogService { get; }
        protected ICategoryService CategoryService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IForumService ForumService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INewsService NewsService { get; }
        protected INopFileProvider FileProvider { get; }
        protected IPageHeadBuilder PageHeadBuilder { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IProductService ProductService { get; }
        protected IProductTagService ProductTagService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected ISitemapGenerator SitemapGenerator { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IThemeContext ThemeContext { get; }
        protected IThemeProvider ThemeProvider { get; }
        protected ITopicService TopicService { get; }
        protected IUrlHelperFactory UrlHelperFactory { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected MediaSettings MediaSettings { get; }
        protected NewsSettings NewsSettings { get; }
        protected SitemapSettings SitemapSettings { get; }
        protected SitemapXmlSettings SitemapXmlSettings { get; }
        protected StoreInformationSettings StoreInformationSettings { get; }
        protected VendorSettings VendorSettings { get; }

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
            IStaticCacheManager staticCacheManager,
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
            BlogSettings = blogSettings;
            CaptchaSettings = captchaSettings;
            CatalogSettings = catalogSettings;
            CommonSettings = commonSettings;
            CustomerSettings = customerSettings;
            DisplayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
            ForumSettings = forumSettings;
            ActionContextAccessor = actionContextAccessor;
            BlogService = blogService;
            CategoryService = categoryService;
            CurrencyService = currencyService;
            CustomerService = customerService;
            ForumService = forumService;
            GenericAttributeService = genericAttributeService;
            HttpContextAccessor = httpContextAccessor;
            LanguageService = languageService;
            LocalizationService = localizationService;
            ManufacturerService = manufacturerService;
            NewsService = newsService;
            FileProvider = fileProvider;
            PageHeadBuilder = pageHeadBuilder;
            PermissionService = permissionService;
            PictureService = pictureService;
            ProductService = productService;
            ProductTagService = productTagService;
            ShoppingCartService = shoppingCartService;
            SitemapGenerator = sitemapGenerator;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            ThemeContext = themeContext;
            ThemeProvider = themeProvider;
            TopicService = topicService;
            UrlHelperFactory = urlHelperFactory;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            LocalizationSettings = localizationSettings;
            NewsSettings = newsSettings;
            SitemapSettings = sitemapSettings;
            SitemapXmlSettings = sitemapXmlSettings;
            StoreInformationSettings = storeInformationSettings;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the number of unread private messages
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of private messages
        /// </returns>
        protected virtual async Task<int> GetUnreadPrivateMessagesAsync()
        {
            var result = 0;
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (ForumSettings.AllowPrivateMessages && !await CustomerService.IsGuestAsync(customer))
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                var privateMessages = await ForumService.GetAllPrivateMessagesAsync(store.Id,
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the logo model
        /// </returns>
        public virtual async Task<LogoModel> PrepareLogoModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var model = new LogoModel
            {
                StoreName = await LocalizationService.GetLocalizedAsync(store, x => x.Name)
            };

            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.StoreLogoPath
                , store, await ThemeContext.GetWorkingThemeNameAsync(), WebHelper.IsCurrentConnectionSecured());
            model.LogoPath = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                var logo = string.Empty;
                var logoPictureId = StoreInformationSettings.LogoPictureId;

                if (logoPictureId > 0)
                    logo = await PictureService.GetPictureUrlAsync(logoPictureId, showDefaultPicture: false);

                if (string.IsNullOrEmpty(logo))
                {
                    //use default logo
                    var pathBase = HttpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
                    var storeLocation = MediaSettings.UseAbsoluteImagePath ? WebHelper.GetStoreLocation() : $"{pathBase}/";
                    logo = $"{storeLocation}Themes/{await ThemeContext.GetWorkingThemeNameAsync()}/Content/images/logo.png";
                }

                return logo;
            });

            return model;
        }

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language selector model
        /// </returns>
        public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var availableLanguages = (await LanguageService
                    .GetAllLanguagesAsync(storeId: store.Id))
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    }).ToList();

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = (await WorkContext.GetWorkingLanguageAsync()).Id,
                AvailableLanguages = availableLanguages,
                UseImages = LocalizationSettings.UseImagesForLanguageSelection
            };

            return model;
        }

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the currency selector model
        /// </returns>
        public virtual async Task<CurrencySelectorModel> PrepareCurrencySelectorModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var availableCurrencies = await (await CurrencyService
                .GetAllCurrenciesAsync(storeId: store.Id))
                .SelectAwait(async x =>
                {
                    //currency char
                    var currencySymbol = !string.IsNullOrEmpty(x.DisplayLocale)
                        ? new RegionInfo(x.DisplayLocale).CurrencySymbol
                        : x.CurrencyCode;

                    //model
                    var currencyModel = new CurrencyModel
                    {
                        Id = x.Id,
                        Name = await LocalizationService.GetLocalizedAsync(x, y => y.Name),
                        CurrencySymbol = currencySymbol
                    };

                    return currencyModel;
                }).ToListAsync();

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = (await WorkContext.GetWorkingCurrencyAsync()).Id,
                AvailableCurrencies = availableCurrencies
            };

            return model;
        }

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax type selector model
        /// </returns>
        public virtual async Task<TaxTypeSelectorModel> PrepareTaxTypeSelectorModelAsync()
        {
            var model = new TaxTypeSelectorModel
            {
                CurrentTaxType = await WorkContext.GetTaxDisplayTypeAsync()
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the header links model
        /// </returns>
        public virtual async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();

            var unreadMessageCount = await GetUnreadPrivateMessagesAsync();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(await LocalizationService.GetResourceAsync("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (ForumSettings.ShowAlertForPM &&
                    !await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, store.Id))
                {
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, true, store.Id);
                    alertMessage = string.Format(await LocalizationService.GetResourceAsync("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                RegistrationType = CustomerSettings.UserRegistrationType,
                IsAuthenticated = await CustomerService.IsRegisteredAsync(customer),
                CustomerName = await CustomerService.IsRegisteredAsync(customer) ? await CustomerService.FormatUsernameAsync(customer) : string.Empty,
                ShoppingCartEnabled = await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
                WishlistEnabled = await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = await CustomerService.IsRegisteredAsync(customer) && ForumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)
            if (customer.HasShoppingCartItems)
            {
                model.ShoppingCartItems = (await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id))
                    .Sum(item => item.Quantity);

                model.WishlistItems = (await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id))
                    .Sum(item => item.Quantity);
            }

            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the admin header links model
        /// </returns>
        public virtual async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedCustomerName = await CustomerService.IsRegisteredAsync(customer) ? await CustomerService.FormatUsernameAsync(customer) : string.Empty,
                IsCustomerImpersonated = WorkContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = PageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the social model
        /// </returns>
        public virtual async Task<SocialModel> PrepareSocialModelAsync()
        {
            var model = new SocialModel
            {
                FacebookLink = StoreInformationSettings.FacebookLink,
                TwitterLink = StoreInformationSettings.TwitterLink,
                YoutubeLink = StoreInformationSettings.YoutubeLink,
                WorkingLanguageId = (await WorkContext.GetWorkingLanguageAsync()).Id,
                NewsEnabled = NewsSettings.Enabled,
            };

            return model;
        }

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the footer model
        /// </returns>
        public virtual async Task<FooterModel> PrepareFooterModelAsync()
        {
            //footer topics
            var store = await StoreContext.GetCurrentStoreAsync();
            var topicModels = await (await TopicService.GetAllTopicsAsync(store.Id))
                    .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                    .SelectAwait(async t => new FooterModel.FooterTopicModel
                    {
                        Id = t.Id,
                        Name = await LocalizationService.GetLocalizedAsync(t, x => x.Title),
                        SeName = await UrlRecordService.GetSeNameAsync(t),
                        IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                        IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                        IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                    }).ToListAsync();

            //model
            var model = new FooterModel
            {
                StoreName = await LocalizationService.GetLocalizedAsync(store, x => x.Name),
                WishlistEnabled = await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = await PermissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = SitemapSettings.SitemapEnabled,
                SearchEnabled = CatalogSettings.ProductSearchEnabled,
                WorkingLanguageId = (await WorkContext.GetWorkingLanguageAsync()).Id,
                BlogEnabled = BlogSettings.Enabled,
                CompareProductsEnabled = CatalogSettings.CompareProductsEnabled,
                ForumEnabled = ForumSettings.ForumsEnabled,
                NewsEnabled = NewsSettings.Enabled,
                RecentlyViewedProductsEnabled = CatalogSettings.RecentlyViewedProductsEnabled,
                NewProductsEnabled = CatalogSettings.NewProductsEnabled,
                DisplayTaxShippingInfoFooter = CatalogSettings.DisplayTaxShippingInfoFooter,
                HidePoweredByNopCommerce = StoreInformationSettings.HidePoweredByNopCommerce,
                IsHomePage = _webHelper.GetStoreLocation().Equals(_webHelper.GetThisPageUrl(false), StringComparison.InvariantCultureIgnoreCase),
                AllowCustomersToApplyForVendorAccount = VendorSettings.AllowCustomersToApplyForVendorAccount,
                AllowCustomersToCheckGiftCardBalance = CustomerSettings.AllowCustomersToCheckGiftCardBalance && CaptchaSettings.Enabled,
                Topics = topicModels,
                DisplaySitemapFooterItem = DisplayDefaultFooterItemSettings.DisplaySitemapFooterItem,
                DisplayContactUsFooterItem = DisplayDefaultFooterItemSettings.DisplayContactUsFooterItem,
                DisplayProductSearchFooterItem = DisplayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
                DisplayNewsFooterItem = DisplayDefaultFooterItemSettings.DisplayNewsFooterItem,
                DisplayBlogFooterItem = DisplayDefaultFooterItemSettings.DisplayBlogFooterItem,
                DisplayForumsFooterItem = DisplayDefaultFooterItemSettings.DisplayForumsFooterItem,
                DisplayRecentlyViewedProductsFooterItem = DisplayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
                DisplayCompareProductsFooterItem = DisplayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
                DisplayNewProductsFooterItem = DisplayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
                DisplayCustomerInfoFooterItem = DisplayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem,
                DisplayCustomerOrdersFooterItem = DisplayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem,
                DisplayCustomerAddressesFooterItem = DisplayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem,
                DisplayShoppingCartFooterItem = DisplayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
                DisplayWishlistFooterItem = DisplayDefaultFooterItemSettings.DisplayWishlistFooterItem,
                DisplayApplyVendorAccountFooterItem = DisplayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the contact us model
        /// </returns>
        public virtual async Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                model.Email = customer.Email;
                model.FullName = await CustomerService.GetCustomerFullNameAsync(customer);
            }

            model.SubjectEnabled = CommonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnContactUsPage;

            return model;
        }

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the contact vendor model
        /// </returns>
        public virtual async Task<ContactVendorModel> PrepareContactVendorModelAsync(ContactVendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (!excludeProperties)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                model.Email = customer.Email;
                model.FullName = await CustomerService.GetCustomerFullNameAsync(customer);
            }

            model.SubjectEnabled = CommonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnContactUsPage;
            model.VendorId = vendor.Id;
            model.VendorName = await LocalizationService.GetLocalizedAsync(vendor, x => x.Name);

            return model;
        }

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap model
        /// </returns>
        public virtual async Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel)
        {
            if (pageModel == null)
                throw new ArgumentNullException(nameof(pageModel));

            var language = await WorkContext.GetWorkingLanguageAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);
            var store = await StoreContext.GetCurrentStoreAsync();
            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapPageModelKey,
                language, customerRoleIds, store);

            var cachedModel = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                //get URL helper
                var urlHelper = UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);

                var model = new SitemapModel();

                //prepare common items
                var commonGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.General");

                //home page
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await LocalizationService.GetResourceAsync("Homepage"),
                    Url = urlHelper.RouteUrl("Homepage")
                });

                //search
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await LocalizationService.GetResourceAsync("Search"),
                    Url = urlHelper.RouteUrl("ProductSearch")
                });

                //news
                if (NewsSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = await LocalizationService.GetResourceAsync("News"),
                        Url = urlHelper.RouteUrl("NewsArchive")
                    });
                }

                //blog
                if (BlogSettings.Enabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = await LocalizationService.GetResourceAsync("Blog"),
                        Url = urlHelper.RouteUrl("Blog")
                    });
                }

                //forums
                if (ForumSettings.ForumsEnabled)
                {
                    model.Items.Add(new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = await LocalizationService.GetResourceAsync("Forum.Forums"),
                        Url = urlHelper.RouteUrl("Boards")
                    });
                }

                //contact us
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await LocalizationService.GetResourceAsync("ContactUs"),
                    Url = urlHelper.RouteUrl("ContactUs")
                });

                //customer info
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await LocalizationService.GetResourceAsync("Account.MyAccount"),
                    Url = urlHelper.RouteUrl("CustomerInfo")
                });

                //at the moment topics are in general category too
                if (SitemapSettings.SitemapIncludeTopics)
                {
                    var topics = (await TopicService.GetAllTopicsAsync(storeId: store.Id))
                        .Where(topic => topic.IncludeInSitemap);

                    model.Items.AddRange(await topics.SelectAwait(async topic => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = commonGroupTitle,
                        Name = await LocalizationService.GetLocalizedAsync(topic, x => x.Title),
                        Url = urlHelper.RouteUrl("Topic", new { SeName = await UrlRecordService.GetSeNameAsync(topic) })
                    }).ToListAsync());
                }

                //blog posts
                if (SitemapSettings.SitemapIncludeBlogPosts && BlogSettings.Enabled)
                {
                    var blogPostsGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.BlogPosts");
                    var blogPosts = (await BlogService.GetAllBlogPostsAsync(storeId: store.Id))
                        .Where(p => p.IncludeInSitemap);

                    model.Items.AddRange(await blogPosts.SelectAwait(async post => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = blogPostsGroupTitle,
                        Name = post.Title,
                        Url = urlHelper.RouteUrl("BlogPost",
                            new { SeName = await UrlRecordService.GetSeNameAsync(post, post.LanguageId, ensureTwoPublishedLanguages: false) })
                    }).ToListAsync());
                }

                //news
                if (SitemapSettings.SitemapIncludeNews && NewsSettings.Enabled)
                {
                    var newsGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.News");
                    var news = await NewsService.GetAllNewsAsync(storeId: store.Id);
                    model.Items.AddRange(await news.SelectAwait(async newsItem => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = newsGroupTitle,
                        Name = newsItem.Title,
                        Url = urlHelper.RouteUrl("NewsItem",
                            new { SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) })
                    }).ToListAsync());
                }

                //categories
                if (SitemapSettings.SitemapIncludeCategories)
                {
                    var categoriesGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.Categories");
                    var categories = await CategoryService.GetAllCategoriesAsync(storeId: store.Id);
                    model.Items.AddRange(await categories.SelectAwait(async category => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = categoriesGroupTitle,
                        Name = await LocalizationService.GetLocalizedAsync(category, x => x.Name),
                        Url = urlHelper.RouteUrl("Category", new { SeName = await UrlRecordService.GetSeNameAsync(category) })
                    }).ToListAsync());
                }

                //manufacturers
                if (SitemapSettings.SitemapIncludeManufacturers)
                {
                    var manufacturersGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.Manufacturers");
                    var manufacturers = await ManufacturerService.GetAllManufacturersAsync(storeId: store.Id);
                    model.Items.AddRange(await manufacturers.SelectAwait(async manufacturer => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = manufacturersGroupTitle,
                        Name = await LocalizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        Url = urlHelper.RouteUrl("Manufacturer", new { SeName = await UrlRecordService.GetSeNameAsync(manufacturer) })
                    }).ToListAsync());
                }

                //products
                if (SitemapSettings.SitemapIncludeProducts)
                {
                    var productsGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.Products");
                    var products = await ProductService.SearchProductsAsync(0, storeId: store.Id, visibleIndividuallyOnly: true);
                    model.Items.AddRange(await products.SelectAwait(async product => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productsGroupTitle,
                        Name = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                        Url = urlHelper.RouteUrl("Product", new { SeName = await UrlRecordService.GetSeNameAsync(product) })
                    }).ToListAsync());
                }

                //product tags
                if (SitemapSettings.SitemapIncludeProductTags)
                {
                    var productTagsGroupTitle = await LocalizationService.GetResourceAsync("Sitemap.ProductTags");
                    var productTags = await ProductTagService.GetAllProductTagsAsync();
                    model.Items.AddRange(await productTags.SelectAwait(async productTag => new SitemapModel.SitemapItemModel
                    {
                        GroupTitle = productTagsGroupTitle,
                        Name = await LocalizationService.GetLocalizedAsync(productTag, x => x.Name),
                        Url = urlHelper.RouteUrl("ProductsByTag", new { SeName = await UrlRecordService.GetSeNameAsync(productTag) })
                    }).ToListAsync());
                }

                return model;
            });

            //prepare model with pagination
            pageModel.PageSize = Math.Max(pageModel.PageSize, SitemapSettings.SitemapPageSize);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap as string in XML format
        /// </returns>
        public virtual async Task<string> PrepareSitemapXmlAsync(int? id)
        {
            var language = await WorkContext.GetWorkingLanguageAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);
            var store = await StoreContext.GetCurrentStoreAsync();
            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapSeoModelKey,
                id, language, customerRoleIds, store);

            var siteMap = await StaticCacheManager.GetAsync(cacheKey, async () => await SitemapGenerator.GenerateAsync(id));

            return siteMap;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store theme selector model
        /// </returns>
        public virtual async Task<StoreThemeSelectorModel> PrepareStoreThemeSelectorModelAsync()
        {
            var model = new StoreThemeSelectorModel();

            var currentTheme = await ThemeProvider.GetThemeBySystemNameAsync(await ThemeContext.GetWorkingThemeNameAsync());
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme?.SystemName,
                Title = currentTheme?.FriendlyName
            };

            model.AvailableStoreThemes = (await ThemeProvider.GetThemesAsync()).Select(x => new StoreThemeModel
            {
                Name = x.SystemName,
                Title = x.FriendlyName
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the favicon model
        /// </returns>
        public virtual Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync()
        {
            var model = new FaviconAndAppIconsModel
            {
                HeadCode = CommonSettings.FaviconAndAppIconsHeadCode
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the robots.txt file as string
        /// </returns>
        public virtual async Task<string> PrepareRobotsTextFileAsync()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = FileProvider.Combine(FileProvider.MapPath("~/"), "robots.custom.txt");
            if (FileProvider.FileExists(robotsFilePath))
            {
                //the robots.txt file exists
                var robotsFileContent = await FileProvider.ReadAllTextAsync(robotsFilePath, Encoding.UTF8);
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
                    "/*?*returnUrl="
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
                    "/customer/productreviews",
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
                    "/recentlyviewedproducts",
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
                if (SitemapXmlSettings.SitemapXmlEnabled)
                {
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", WebHelper.GetStoreLocation());
                    sb.Append(newLine);
                }

                //host
                sb.AppendFormat("Host: {0}", WebHelper.GetStoreLocation());
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

                if (LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var store = await StoreContext.GetCurrentStoreAsync();
                    //URLs are localizable. Append SEO code
                    foreach (var language in await LanguageService.GetAllLanguagesAsync(storeId: store.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = FileProvider.Combine(FileProvider.MapPath("~/"), "robots.additions.txt");
                if (FileProvider.FileExists(robotsAdditionsFile))
                {
                    var robotsFileContent = await FileProvider.ReadAllTextAsync(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}