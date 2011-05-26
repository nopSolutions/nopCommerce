using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Home;

namespace Nop.Web.Controllers
{
    public class HomeController : BaseNopController
    {
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;

        private readonly UserSettings _userSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;

        public HomeController(ILanguageService languageService, 
            ICurrencyService currencyService, ICustomerService customerService,
            ILocalizationService localizationService,
            IWorkContext workContext, IAuthenticationService authenticationService,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            UserSettings userSettings, ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings, CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings, EmailAccountSettings emailAccountSettings)
        {
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;

            this._userSettings = userSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
        }

        public ActionResult Index()
        {
            return View();
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService.GetAllLanguages().Select(x=> x.ToModel()).ToList();
            return PartialView(model);
        }

        public ActionResult LanguageSelected(int customerlanguage)
        {
            var language = _languageService.GetLanguageById(customerlanguage);
            if(language != null)
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
            };

            return PartialView(model);
        }

        //contact us page

        public ActionResult ContactUs()
        {
            var model = new ContactUsModel();
            model.Email = _workContext.CurrentCustomer != null ? _workContext.CurrentCustomer.GetDefaultUserAccountEmail() : null;
            model.FullName = _workContext.CurrentCustomer != null ? string.Format("{0} {1}", _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LastName)) : null;
            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]
        [FormValueRequired("send-email")]
        public ActionResult ContactUsSend(ContactUsModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = string.Format("{0}. {1}", _storeInformationSettings.StoreName, "Contact us");
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                
                
                string from = email;
                string fromName = fullName;

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                //TODO uncomment below
                //required for some SMTP servers
                //if (this.SettingManager.GetSettingValueBoolean("Email.UseSystemEmailForContactUsForm"))
                //{
                //    from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                //    body = string.Format("<b>From</b>: {0} - {1}<br /><br />{2}", Server.HtmlEncode(fullName), Server.HtmlEncode(email), body);
                //}
                var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
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


                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

    }
}
