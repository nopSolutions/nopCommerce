using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
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
        private readonly IAuthenticationService _authenticationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;

        private readonly UserSettings _userSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;

        public CommonController(ICategoryService categoryService, IProductService productService,
            IManufacturerService manufacturerService, ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService, ILocalizationService localizationService,
            IWorkContext workContext, IAuthenticationService authenticationService,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator, 
            UserSettings userSettings, ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings, CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings, EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings, BlogSettings blogSettings)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;

            this._userSettings = userSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService.GetAllLanguages().Select(x => x.ToModel()).ToList();
            return PartialView(model);
        }

        public ActionResult LanguageSelected(int customerlanguage)
        {
            var language = _languageService.GetLanguageById(customerlanguage);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService.GetAllLanguages().Select(x => x.ToModel()).ToList();
            return PartialView("LanguageSelector", model);
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

        //header
        [ChildActionOnly]
        public ActionResult Header()
        {
            var user = _authenticationService.GetAuthenticatedUser();
            var customer = _workContext.CurrentCustomer;
            var model = new HeaderModel()
            {
                IsAuthenticated = user != null,
                CustomerEmailUsername = user != null ? (_userSettings.UsernamesEnabled ? user.Username : user.Email) : "",
                //TODO uncomment later
                //DisplayAdminLink = customer != null && customer.IsAdmin(),
                DisplayAdminLink = true,
                ShoppingCartItems = customer != null ? customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList().GetTotalProducts() : 0,
                WishlistEnabled = _shoppingCartSettings.WishlistEnabled,
                WishlistItems = customer != null ? customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList().GetTotalProducts() : 0,
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
            };

            return PartialView(model);
        }

        //contact us page
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel();
            model.Email = _workContext.CurrentCustomer != null ? _workContext.CurrentCustomer.GetDefaultUserAccountEmail() : null;
            model.FullName = _workContext.CurrentCustomer != null ? _workContext.CurrentCustomer.GetFullName() : null;
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
                var products = _productService.SearchProducts(0, 0, null, null, null, 0, 0, null, false, 0, null,
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
    }
}
