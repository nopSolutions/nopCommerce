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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Core.Caching;
using System.Web.Security;

namespace Nop.Services
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class AuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly CustomerService _customerService;

        private Customer _signedInUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="customerService">Customer service</param>
        public AuthenticationService(HttpContextBase httpContext,
            CustomerService customerService)
        {
            this._httpContext = httpContext;
            this._customerService = customerService;

            //TODO set correct timespan
            ExpirationTimeSpan = TimeSpan.FromHours(6);
        }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public void SignIn(Customer customer, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var userData = Convert.ToString(customer.Id);

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                customer.Email,
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
            _signedInUser = customer;
        }

        public void SignOut()
        {
            _signedInUser = null;
            FormsAuthentication.SignOut();
        }

        public Customer GetAuthenticatedCustomer()
        {
            if (_signedInUser != null)
                return _signedInUser;

            if (!_httpContext.Request.IsAuthenticated || !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var userData = formsIdentity.Ticket.UserData;
            int userId;
            if (!int.TryParse(userData, out userId))
                return null;
            return _customerService.GetCustomerById(userId);
        }
    }
}