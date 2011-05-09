using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Services;
using Nop.Services.Customers;
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
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly UserSettings _userSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public HomeController(ILanguageService languageService, IWorkContext workContext,
            IAuthenticationService authenticationService,
            UserSettings userSettings, ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._languageService = languageService;
            this._authenticationService = authenticationService;
            this._userSettings = userSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            var avaibleLanguages = _languageService.GetAllLanguages();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvaibleLanguages = avaibleLanguages.Select(x=> x.ToModel()).ToList();
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
            var avaibleLanguages = _languageService.GetAllLanguages();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvaibleLanguages = avaibleLanguages.Select(x => x.ToModel()).ToList();
            model.IsAjaxRequest = true;
            return PartialView("LanguageSelector", model);
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
