//Contributor:  Nicholas Mayne

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// External authorizer helper
    /// </summary>
    public static partial class ExternalAuthorizerHelper
    {
#if NET451
        private static ISession GetSession()
        {
            return EngineContext.Current.Resolve<IHttpContextAccessor>().HttpContext?.Session;
        }

        public static void StoreParametersForRoundTrip(OpenAuthenticationParameters parameters)
        {
            var session = GetSession();
            session.Set("nop.externalauth.parameters", parameters);
        }
        public static OpenAuthenticationParameters RetrieveParametersFromRoundTrip(bool removeOnRetrieval)
        {
            var session = GetSession();
            var parameters = session.Get<OpenAuthenticationParameters>("nop.externalauth.parameters");
            if (parameters != null && removeOnRetrieval)
                RemoveParameters();

            return parameters;
        }

        public static void RemoveParameters()
        {
            var session = GetSession();
            session.Remove("nop.externalauth.parameters");
        }

        public static void AddErrorsToDisplay(string error)
        {
            var session = GetSession();
            var errors = session.Get<IList<string>>("nop.externalauth.errors");
            if (errors == null)
            {
                errors = new List<string>();
                session.Set("nop.externalauth.errors", errors);
            }
            errors.Add(error);
        }

        public static IList<string> RetrieveErrorsToDisplay(bool removeOnRetrieval)
        {
            var session = GetSession();
            var errors = session.Get<IList<string>>("nop.externalauth.errors");
            if (errors != null && removeOnRetrieval)
                session.Remove("nop.externalauth.errors");
            return errors;
        }
#endif
    }
}