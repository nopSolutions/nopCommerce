using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// External authorizer helper
    /// </summary>
    public static partial class ExternalAuthorizerHelper
    {
        #region Constants

        /// <summary>
        /// Key for store external authentication errors to session
        /// </summary>
        private const string EXTERNAL_AUTHENTICATION_ERRORS = "nop.externalauth.errors";

        #endregion

        #region Methods

        public static void AddErrorsToDisplay(string error)
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext.Session;
            var errors = session.Get<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS) ?? new List<string>();
            errors.Add(error);
            session.Set(EXTERNAL_AUTHENTICATION_ERRORS, errors);
        }

        public static IList<string> RetrieveErrorsToDisplay()
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext.Session;
            var errors = session.Get<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS);

            if (errors != null)
                session.Remove(EXTERNAL_AUTHENTICATION_ERRORS);

            return errors;
        }

        #endregion
    }
}