using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Extensions;
using Nop.Web.Models;
using Nop.Web.Models.Home;

namespace Nop.Web.Controllers
{
    public class HomeController : BaseNopController
    {
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly UserSettings _userSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;

        public HomeController(ILanguageService languageService, 
            ICurrencyService currencyService, ICustomerService customerService,
            IWorkContext workContext, IAuthenticationService authenticationService,
            UserSettings userSettings, ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings)
        {
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
            this._userSettings = userSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._taxSettings = taxSettings;
        }

        public ActionResult Index()
        {
            return View();
        }

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

    }
}
