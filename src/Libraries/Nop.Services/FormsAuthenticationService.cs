
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

        private User _cachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            IUserService userService)
        {
            this._httpContext = httpContext;
            this._userService = userService;

            //TODO set correct timespan
            ExpirationTimeSpan = TimeSpan.FromHours(6);
        }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.Now.ToLocalTime();
            var userData = Convert.ToString(user.Username);

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.Username,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                userData,
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
            var userData = formsIdentity.Ticket.UserData;

            var username = userData;
            if (String.IsNullOrWhiteSpace(username))
                return null;
            _cachedUser = _userService.GetUserByUsername(username);
            return _cachedUser;
        }
    }
}