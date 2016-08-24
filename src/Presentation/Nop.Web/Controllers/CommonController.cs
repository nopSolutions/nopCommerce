using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Topics;

namespace Nop.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IForumService _forumservice;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPictureService _pictureService;

        private readonly CustomerSettings _customerSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Constructors

        public CommonController(ICategoryService categoryService,
            IProductService productService,
            IManufacturerService manufacturerService,
            ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ICacheManager cacheManager,
            ICustomerActivityService customerActivityService,
            IVendorService vendorService,
            IPageHeadBuilder pageHeadBuilder,
            IPictureService pictureService,
            CustomerSettings customerSettings,
            TaxSettings taxSettings,
            CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            BlogSettings blogSettings,
            NewsSettings newsSettings,
            ForumSettings forumSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            VendorSettings vendorSettings)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._forumservice = forumService;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;
            this._vendorService = vendorService;
            this._pageHeadBuilder = pageHeadBuilder;
            this._pictureService = pictureService;


            this._customerSettings = customerSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._newsSettings = newsSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.CurrentCustomer;
            if (_forumSettings.AllowPrivateMessages && !customer.IsGuest())
            {
                var privateMessages = _forumservice.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
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

        //page not found
        public ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //logo
        [ChildActionOnly]
        public ActionResult Logo()
        {
            var model = new LogoModel
            {
                StoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name)
            };

            var cacheKey = string.Format(ModelCacheEventConsumer.STORE_LOGO_PATH, _storeContext.CurrentStore.Id, _themeContext.WorkingThemeName, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _storeInformationSettings.LogoPictureId;
                if (logoPictureId > 0)
                {
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);
                }
                if (String.IsNullOrEmpty(logo))
                {
                    //use default logo
                    logo = string.Format("{0}Themes/{1}/Content/images/logo.png", _webHelper.GetStoreLocation(), _themeContext.WorkingThemeName);
                }
                return logo;
            });

            return PartialView(model);
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentStore.Id), () =>
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

            if (model.AvailableLanguages.Count == 1)
                Content("");

            return PartialView(model);
        }
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //language part in URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Request.ApplicationPath;
                if (returnUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //already localized URL
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                }
                returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
            }
            return Redirect(returnUrl);
        }

        //currency
        [ChildActionOnly]
        public ActionResult CurrencySelector()
        {
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id), () =>
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
                            Name = x.GetLocalized(y => y.Name),
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

            if (model.AvailableCurrencies.Count == 1)
                Content("");

            return PartialView(model);
        }
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult SetCurrency(int customerCurrency, string returnUrl = "")
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //tax type
        [ChildActionOnly]
        public ActionResult TaxTypeSelector()
        {
            if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                return Content("");

            var model = new TaxTypeSelectorModel
            {
                CurrentTaxType = _workContext.TaxDisplayType
            };

            return PartialView(model);
        }
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult SetTaxType(int customerTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //footer
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }

        //header links
        [ChildActionOnly]
        public ActionResult HeaderLinks()
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
                    !customer.GetAttribute<bool>(SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, _storeContext.CurrentStore.Id))
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, true, _storeContext.CurrentStore.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = customer.IsRegistered() && _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
            //performance optimization (use "HasShoppingCartItems" property)
            if (customer.HasShoppingCartItems)
            {
                model.ShoppingCartItems = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList()
                    .GetTotalProducts();
                model.WishlistItems = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList()
                    .GetTotalProducts();
            }

            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult AdminHeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedCustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return PartialView(model);
        }


        //social
        [ChildActionOnly]
        public ActionResult Social()
        {
            //model
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                GooglePlusLink = _storeInformationSettings.GooglePlusLink,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                NewsEnabled = _newsSettings.Enabled,
            };

            return PartialView(model);
        }


        //footer
        [ChildActionOnly]
        public ActionResult Footer()
        {
            //footer topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_FOOTER_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                .Select(t => new FooterModel.FooterTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName(),
                    IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                })
                .ToList()
            );

            //model
            var model = new FooterModel
            {
                StoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _commonSettings.SitemapEnabled,
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
                Topics = cachedTopicModel
            };

            return PartialView(model);
        }


        //contact us page
        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel
            {
                Email = _workContext.CurrentCustomer.Email,
                FullName = _workContext.CurrentCustomer.GetFullName(),
                SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage
            };
            return View(model);
        }
        [HttpPost, ActionName("ContactUs")]
        [PublicAntiForgery]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        public ActionResult ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = _commonSettings.SubjectFieldOnContactUsForm ?
                    model.Subject :
                    string.Format(_localizationService.GetResource("ContactUs.EmailSubject"), _storeContext.CurrentStore.GetLocalized(x => x.Name));

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                string from;
                string fromName;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = emailAccount.Email,
                    ToName = emailAccount.DisplayName,
                    ReplyTo = email,
                    ReplyToName = fullName,
                    Priority = QueuedEmailPriority.High,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            return View(model);
        }
        //contact vendor page
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ContactVendor(int vendorId)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            var model = new ContactVendorModel
            {
                Email = _workContext.CurrentCustomer.Email,
                FullName = _workContext.CurrentCustomer.GetFullName(),
                SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage,
                VendorId = vendor.Id,
                VendorName = vendor.GetLocalized(x => x.Name)
            };
            return View(model);
        }
        [HttpPost, ActionName("ContactVendor")]
        [PublicAntiForgery]
        [CaptchaValidator]
        public ActionResult ContactVendorSend(ContactVendorModel model, bool captchaValid)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            model.VendorName = vendor.GetLocalized(x => x.Name);

            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;

                string subject = _commonSettings.SubjectFieldOnContactUsForm ?
                    model.Subject :
                    string.Format(_localizationService.GetResource("ContactVendor.EmailSubject"), _storeContext.CurrentStore.GetLocalized(x => x.Name));


                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                string from;
                string fromName;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = vendor.Email,
                    ToName = vendor.Name,
                    ReplyTo = email,
                    ReplyToName = fullName,
                    Priority = QueuedEmailPriority.High,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent");

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            return View(model);
        }

        //sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Sitemap()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            string cacheKey = string.Format(ModelCacheEventConsumer.SITEMAP_PAGE_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new SitemapModel
                {
                    BlogEnabled = _blogSettings.Enabled,
                    ForumEnabled = _forumSettings.ForumsEnabled,
                    NewsEnabled = _newsSettings.Enabled,
                };
                //categories
                if (_commonSettings.SitemapIncludeCategories)
                {
                    var categories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                    model.Categories = categories.Select(x => x.ToModel()).ToList();
                }
                //manufacturers
                if (_commonSettings.SitemapIncludeManufacturers)
                {
                    var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
                    model.Manufacturers = manufacturers.Select(x => x.ToModel()).ToList();
                }
                //products
                if (_commonSettings.SitemapIncludeProducts)
                {
                    //limit product to 200 until paging is supported on this page
                    var products = _productService.SearchProducts(storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        pageSize: 200);
                    model.Products = products.Select(product => new ProductOverviewModel
                    {
                        Id = product.Id,
                        Name = product.GetLocalized(x => x.Name),
                        ShortDescription = product.GetLocalized(x => x.ShortDescription),
                        FullDescription = product.GetLocalized(x => x.FullDescription),
                        SeName = product.GetSeName(),
                    }).ToList();
                }

                //topics
                var topics = _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                    .Where(t => t.IncludeInSitemap)
                    .ToList();
                model.Topics = topics.Select(topic => new TopicModel
                {
                    Id = topic.Id,
                    SystemName = topic.SystemName,
                    IncludeInSitemap = topic.IncludeInSitemap,
                    IsPasswordProtected = topic.IsPasswordProtected,
                    Title = topic.GetLocalized(x => x.Title),
                })
                .ToList();
                return model;
            });

            return View(cachedModel);
        }

        //SEO sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        //available even when a store is closed
        [StoreClosed(true)]
        public ActionResult SitemapXml()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            string cacheKey = string.Format(ModelCacheEventConsumer.SITEMAP_SEO_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(this.Url));
            return Content(siteMap, "text/xml");
        }

        //store theme
        [ChildActionOnly]
        public ActionResult StoreThemeSelector()
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingThemeName);
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                .Select(x => new StoreThemeModel
                {
                    Name = x.ThemeName,
                    Title = x.ThemeTitle
                })
                .ToList();
            return PartialView(model);
        }
        public ActionResult SetStoreTheme(string themeName, string returnUrl = "")
        {
            _themeContext.WorkingThemeName = themeName;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            //try loading a store specific favicon
            var faviconFileName = string.Format("favicon-{0}.ico", _storeContext.CurrentStore.Id);
            var localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return Content("");
                }
            }

            var model = new FaviconModel
            {
                FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName
            };
            return PartialView(model);
        }

        //EU Cookie law
        [ChildActionOnly]
        public ActionResult EuCookieLaw()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if (_workContext.CurrentCustomer.IsSearchEngineAccount())
                return Content("");

            if (_workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.EuCookieLawAccepted, _storeContext.CurrentStore.Id))
                //already accepted
                return Content("");

            //ignore notification?
            //right now it's used during logout so popup window is not displayed twice
            if (TempData["nop.IgnoreEuCookieLawWarning"] != null && Convert.ToBoolean(TempData["nop.IgnoreEuCookieLawWarning"]))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.EuCookieLawAccepted, true, _storeContext.CurrentStore.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public ActionResult RobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            string robotsFilePath = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.custom.txt");
            if (System.IO.File.Exists(robotsFilePath))
            {
                //the robots.txt file exists
                string robotsFileContent = System.IO.File.ReadAllText(robotsFilePath);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/bin/",
                    "/content/files/",
                    "/content/files/exportimport/",
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
                    "/cart",
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
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfileproductattribute",
                    "/uploadfilecheckoutattribute",
                    "/wishlist",
                };


                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _storeContext.CurrentStore.Url, language.UniqueSeoCode);
                        sb.Append(newLine);
                    }
                }
                else
                {
                    //localizable paths (without SEO code)
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", _storeContext.CurrentStore.Url);
                    sb.Append(newLine);
                }

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
                            sb.AppendFormat("Disallow: {0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                string robotsAdditionsFile = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.additions.txt");
                if (System.IO.File.Exists(robotsAdditionsFile))
                {
                    string robotsFileContent = System.IO.File.ReadAllText(robotsAdditionsFile);
                    sb.Append(robotsFileContent);
                }
            }


            Response.ContentType = MimeTypes.TextPlain;
            Response.Write(sb.ToString());
            return null;
        }

        public ActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [StoreClosed(true)]
        public ActionResult StoreClosed()
        {
            return View();
        }

        #endregion
    }
}
