//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Services.Authentication.External
{
    public partial class AuthorizeState
    {
        public IList<string> Errors { get; set; }

        private readonly string _returnUrl;
        
        public AuthorizeState(string returnUrl, OpenAuthenticationStatus openAuthenticationStatus)
        {
            Errors = new List<string>();
            _returnUrl = returnUrl;
            AuthenticationStatus = openAuthenticationStatus;

            //in way SEO friendly language URLs will be persisted
            if (AuthenticationStatus == OpenAuthenticationStatus.Authenticated)
                Result = new RedirectResult(!string.IsNullOrEmpty(_returnUrl) ? _returnUrl : "~/");
        }

        public AuthorizeState(string returnUrl, AuthorizationResult authorizationResult)
            : this(returnUrl, authorizationResult.Status)
        {
            Errors = authorizationResult.Errors;
        }

        public OpenAuthenticationStatus AuthenticationStatus { get; private set; }
        
        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        public ActionResult Result { get; set; }
    }
}