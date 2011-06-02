
using System;
using System.Web;
using System.Web.Security;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Services
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;

        private User _cachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        /// <param name="userSettings">User settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            IUserService userService, UserSettings userSettings)
        {
            this._httpContext = httpContext;
            this._userService = userService;
            this._userSettings = userSettings;
            //TODO set correct timespan
            ExpirationTimeSpan = TimeSpan.FromHours(24);
        }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.Now.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                _userSettings.UsernamesEnabled ? user.Username : user.Email,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                _userSettings.UsernamesEnabled ? user.Username : user.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;
        }

        public void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }

        public User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var usernameOrEmail = formsIdentity.Ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;
            var user =_userSettings.UsernamesEnabled 
                ? _userService.GetUserByUsername(usernameOrEmail)
                : _userService.GetUserByEmail(usernameOrEmail);
            if (user != null && user.IsApproved && !user.IsLockedOut)
                _cachedUser = user;
            return _cachedUser;
        }
    }
}