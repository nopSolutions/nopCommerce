using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Models;
using Nop.Web.Models.Customer;

namespace Nop.Web.Controllers
{
    public class CustomerController : BaseNopController
    {
        private IAuthenticationService _authenticationService;
        private IUserService _userService;
        private UserSettings _userSettings;

        public CustomerController(IAuthenticationService authenticationService,
            IUserService userService, UserSettings userSettings)
        {
            this._authenticationService = authenticationService;
            this._userService = userService;
            this._userSettings = userSettings;
        }

        public ActionResult Login()
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_userService.ValidateUser(_userSettings.UsernamesEnabled ? model.UserName : model.Email, model.Password))
                {
                    //TODO migrate shopping cart

                    var user = _userSettings.UsernamesEnabled ? _userService.GetUserByUsername(model.UserName) : _userService.GetUserByEmail(model.Email);
                    _authenticationService.SignIn(user, model.RememberMe);

                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The credentials provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            return View(model);
        }

        public ActionResult Logout()
        {
            _authenticationService.SignOut();

            return this.RedirectToAction("Index", "Home");
        }
    }
}
