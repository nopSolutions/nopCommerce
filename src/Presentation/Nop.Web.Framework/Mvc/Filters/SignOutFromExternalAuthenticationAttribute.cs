using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Authentication;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that sign out from the external authentication scheme
    /// </summary>
    public class SignOutFromExternalAuthenticationAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public SignOutFromExternalAuthenticationAttribute() : base(typeof(SignOutFromExternalAuthenticationFilter))
        {
        }

        #region Nested filter

        /// <summary>
        /// Represents a filter that sign out from the external authentication scheme
        /// </summary>
        private class SignOutFromExternalAuthenticationFilter : IAuthorizationFilter
        {
            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                var authenticationManager = filterContext?.HttpContext?.Authentication;
                if (authenticationManager == null)
                    return;

                //sign out from the external authentication scheme
                var userPrincipal = authenticationManager.AuthenticateAsync(NopCookieAuthenticationDefaults.ExternalAuthenticationScheme).Result;
                var userIdentity = userPrincipal?.Identities?.FirstOrDefault(identity => identity.IsAuthenticated);
                if (userIdentity != null)
                    authenticationManager.SignOutAsync(NopCookieAuthenticationDefaults.ExternalAuthenticationScheme).Wait();
            }

            #endregion
        }

        #endregion
    }
}