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
        /// Key for store external authentication parameters to session
        /// </summary>
        private const string EXTERNAL_AUTHENTICATION_PARAMETERS = "nop.externalauth.parameters";

        /// <summary>
        /// Key for store external authentication errors to session
        /// </summary>
        private const string EXTERNAL_AUTHENTICATION_ERRORS = "nop.externalauth.errors";

        #endregion

        #region Methods

        public static void StoreParametersForRoundTrip(ExternalAuthenticationParameters parameters)
        {
            EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session?.Set(EXTERNAL_AUTHENTICATION_PARAMETERS, parameters);
        }

        public static ExternalAuthenticationParameters RetrieveParametersFromRoundTrip(bool removeOnRetrieval)
        {
            var parameters = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session?
                .Get<ExternalAuthenticationParameters>(EXTERNAL_AUTHENTICATION_PARAMETERS);

            if (parameters != null && removeOnRetrieval)
                RemoveParameters();

            return parameters;
        }

        public static void RemoveParameters()
        {
            EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session?.Remove(EXTERNAL_AUTHENTICATION_PARAMETERS);
        }

        public static void AddErrorsToDisplay(string error)
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session;
            var errors = session?.Get<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS) ?? new List<string>();
            errors.Add(error);
            session?.Set(EXTERNAL_AUTHENTICATION_ERRORS, errors);
        }

        public static IList<string> RetrieveErrorsToDisplay(bool removeOnRetrieval)
        {
            var session = EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session;
            var errors = session?.Get<IList<string>>(EXTERNAL_AUTHENTICATION_ERRORS);

            if (errors != null && removeOnRetrieval)
                session.Remove(EXTERNAL_AUTHENTICATION_ERRORS);

            return errors;
        }

        #endregion
    }
}