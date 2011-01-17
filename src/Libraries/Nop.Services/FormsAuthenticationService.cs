//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

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
    public partial class AuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly UserService _userService;

        private User _cachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        public AuthenticationService(HttpContextBase httpContext,
            UserService userService)
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