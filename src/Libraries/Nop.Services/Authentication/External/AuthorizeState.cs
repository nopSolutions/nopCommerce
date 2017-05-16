//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using System.Linq;
#if NET451
using System.Web.Mvc;
#endif

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Authorize state
    /// </summary>
    public partial class AuthorizeState
    {
        public AuthorizeState(string returnUrl, OpenAuthenticationStatus openAuthenticationStatus)
        {
            Errors = new List<string>();
            AuthenticationStatus = openAuthenticationStatus;

#if NET451
            //in way SEO friendly language URLs will be persisted
            if (AuthenticationStatus == OpenAuthenticationStatus.Authenticated)
                Result = new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/");
#endif
        }

        public AuthorizeState(string returnUrl, AuthorizationResult authorizationResult)
            : this(returnUrl, authorizationResult.Status)
        {
            Errors = authorizationResult.Errors;
        }

        public OpenAuthenticationStatus AuthenticationStatus { get; private set; }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (!this.Errors.Any()); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }

#if NET451
        /// <summary>
        /// Result
        /// </summary>
        public IActionResult Result { get; set; }
#endif
    }
}