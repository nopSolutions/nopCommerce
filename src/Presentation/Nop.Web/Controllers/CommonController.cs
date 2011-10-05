using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    public class CommonController : BaseNopController
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IForumService _forumservice;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

        private readonly CustomerSettings _customerSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;

        public CommonController(ICategoryService categoryService, IProductService productService,
            IManufacturerService manufacturerService, ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService, ILocalizationService localizationService,
            IWorkContext workContext,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator, IThemeContext themeContext,
            IThemeProvider themeProvider, IForumService forumService,
            ICustomerService customerService, IWebHelper webHelper,
            IPermissionService permissionService, 
            CustomerSettings customerSettings, ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings, CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings, EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings, BlogSettings blogSettings, ForumSettings forumSettings,
            LocalizationSettings localizationSettings)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._forumservice = forumService;
            this._customerService = customerService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;

            this._customerSettings = customerSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel()
            {
                CurrentLanguage = _workContext.WorkingLanguage.ToModel(),
                AvailableLanguages = _languageService.GetAllLanguages().Select(x => x.ToModel()).ToList(),
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };
            return PartialView(model);
        }

        public ActionResult SetLanguage(int langid)
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
            }


            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Request.ApplicationPath;
                if (HttpContext.Request.UrlReferrer != null)
                {
                    string redirectUrl = HttpContext.Request.UrlReferrer.PathAndQuery;
                    if (redirectUrl.IsLocalizedUrl(applicationPath, true))
                    {
                        //already localized URL
                        redirectUrl = redirectUrl.RemoveLocalizedPathFromRawUrl(applicationPath);
                    }
                    redirectUrl = redirectUrl.AddLocalizedPathToRawUrl(applicationPath, _workContext.WorkingLanguage);
                    return Redirect(redirectUrl);
                }
                else
                {
                    string redirectUrl = Url.Action("Index", "Home");
                    redirectUrl = redirectUrl.AddLocalizedPathToRawUrl(applicationPath, _workContext.WorkingLanguage);
                    return Redirect(redirectUrl);
                }
            }
            else
            {
                //TODO: URL referrer is null in IE 8. Fix it
                if (HttpContext.Request.UrlReferrer != null)
                {
                    return Redirect(HttpContext.Request.UrlReferrer.PathAndQuery);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        //currency
        [ChildActionOnly]
        public ActionResult CurrencySelector()
        {
            var model = new CurrencySelectorModel();
            model.CurrentCurrency = _workContext.WorkingCurrency.ToModel();
            model.AvailableCurrencies = _currencyService.GetAllCurrencies().Select(x => x.ToModel()).ToList();
            return PartialView(model);
        }

        public ActionResult CurrencySelected(int customerCurrency)
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
            {
                _workContext.WorkingCurrency = currency;
            }
            var model = new CurrencySelectorModel();
            model.CurrentCurrency = _workContext.WorkingCurrency.ToModel();
            model.AvailableCurrencies = _currencyService.GetAllCurrencies().Select(x => x.ToModel()).ToList();
            return PartialView("CurrencySelector", model);
        }

        //tax type
        [ChildActionOnly]
        public ActionResult TaxTypeSelector()
        {
            var model = new TaxTypeSelectorModel();
            model.Enabled = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.CurrentTaxType = _workContext.TaxDisplayType;
            return PartialView(model);
        }

        public ActionResult TaxTypeSelected(int customerTaxType)
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            var model = new TaxTypeSelectorModel();
            model.Enabled = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.CurrentTaxType = _workContext.TaxDisplayType;
            return PartialView("TaxTypeSelector", model);
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
                    !customer.GetAttribute<bool>(SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages))
                {
                    _customerService.SaveCustomerAttribute<bool>(customer, SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, true);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel()
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                ShoppingCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList().GetTotalProducts(),
                WishlistEnabled = _shoppingCartSettings.WishlistEnabled,
                WishlistItems = customer != null ? customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList().GetTotalProducts() : 0,
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };

            return PartialView(model);
        }

        [NonAction]
        private int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.CurrentCustomer;
            if (_forumSettings.AllowPrivateMessages && !customer.IsGuest())
            {
                var privateMessages = _forumservice.GetAllPrivateMessages(0, customer.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        //footer
        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterModel()
            {
                StoreName = _storeInformationSettings.StoreName
            };

            return PartialView(model);
        }

        //menu
        [ChildActionOnly]
        public ActionResult Menu()
        {
            var model = new MenuModel()
            {
                RecentlyAddedProductsEnabled = _catalogSettings.RecentlyAddedProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };

            return PartialView(model);
        }

        //info block
        [ChildActionOnly]
        public ActionResult InfoBlock()
        {
            var model = new InfoBlockModel()
            {
                RecentlyAddedProductsEnabled = _catalogSettings.RecentlyAddedProductsEnabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                SitemapEnabled = _commonSettings.SitemapEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };

            return PartialView(model);
        }

        //contact us page
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel()
            {
                Email = _workContext.CurrentCustomer.Email,
                FullName = _workContext.CurrentCustomer.GetFullName()
            };
            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]
        [FormValueRequired("send-email")]
        public ActionResult ContactUsSend(ContactUsModel model)
        {
            //ajax form
            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = string.Format("{0}. {1}", _storeInformationSettings.StoreName, "Contact us");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);

                string from = null;
                string fromName = null;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<b>From</b>: {0} - {1}<br /><br />{2}", Server.HtmlEncode(fullName), Server.HtmlEncode(email), body);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail()
                    {
                        From = from,
                        FromName = fromName,
                        To = emailAccount.Email,
                        ToName = emailAccount.DisplayName,
                        Priority = 5,
                        Subject = subject,
                        Body = body,
                        CreatedOnUtc = DateTime.UtcNow,
                        EmailAccountId = emailAccount.Id
                    });
                
                return Content(_localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent"));
            }

            return Content(ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage);
        }

        //sitemap page
        public ActionResult Sitemap()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToAction("Index", "Home");

            var model = new SitemapModel();
            if (_commonSettings.SitemapIncludeCategories)
            {
                var categories = _categoryService.GetAllCategories();
                model.Categories = categories.Select(x => x.ToModel()).ToList();
            }
            if (_commonSettings.SitemapIncludeManufacturers)
            {
                var manufacturers = _manufacturerService.GetAllManufacturers();
                model.Manufacturers = manufacturers.Select(x => x.ToModel()).ToList();
            }
            if (_commonSettings.SitemapIncludeProducts)
            {
                //limit product to 200 until paging is supported on this page
                var products = _productService.SearchProducts(0, 0, null, null, null, 0, null, false, 0, null,
                     ProductSortingEnum.Position, 0, 200);
                model.Products = products.Select(x => x.ToModel()).ToList();
            }
            if (_commonSettings.SitemapIncludeTopics)
            {
                var topics = _topicService.GetAllTopics().ToList().FindAll(t => t.IncludeInSitemap);
                model.Topics = topics.Select(x => x.ToModel()).ToList();
            }
            return View(model);
        }

        //SEO sitemap page
        public ActionResult SitemapSeo()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToAction("Index", "Home");

            string siteMap = _sitemapGenerator.Generate();
            return Content(siteMap, "text/xml");
        }

        //store theme
        [ChildActionOnly]
        public ActionResult StoreThemeSelector()
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingTheme);
            model.CurrentStoreTheme = new StoreThemeModel()
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                .Select(x =>
                {
                    return new StoreThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();
            return PartialView(model);
        }

        public ActionResult StoreThemeSelected(string themeName)
        {
            _themeContext.WorkingTheme = themeName;
            
            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingTheme);
            model.CurrentStoreTheme = new StoreThemeModel()
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                .Select(x =>
                {
                    return new StoreThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();
            return PartialView("StoreThemeSelector", model);
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            var model = new FaviconModel()
            {
                Uploaded = System.IO.File.Exists(Request.PhysicalApplicationPath + "favicon.ico"),
                FaviconUrl = _webHelper.GetStoreLocation() + "favicon.ico"
            };
            
            return PartialView(model);
        }
    }
}
