//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Services.Authentication.External
{
    public static partial class ExternalAuthorizerHelper
    {
        public static OpenAuthenticationParameters RetrieveParametersFromRoundTrip(bool removeOnRetrieval)
        {
            var parameters = HttpContext.Current.Session["nop.externalauth.parameters"];
            if (parameters != null && removeOnRetrieval)
                RemoveParameters();

            return parameters as OpenAuthenticationParameters;
        }

        public static void RemoveParameters()
        {
            HttpContext.Current.Session.Remove("nop.externalauth.parameters");
        }

        public static void AddErrorsToDisplay(string error)
        {
            var errors = HttpContext.Current.Session["nop.externalauth.errors"] as IList<string>;
            if (errors == null)
            {
                errors = new List<string>();
                HttpContext.Current.Session.Add("nop.externalauth.errors", errors);
            }
            errors.Add(error);
        }

        public static IList<string> RetrieveErrorsToDisplay(bool removeOnRetrieval)
        {
            var errors = HttpContext.Current.Session["nop.externalauth.errors"] as IList<string>;
            if (errors != null && removeOnRetrieval)
                HttpContext.Current.Session.Remove("nop.externalauth.errors");
            return errors;
        }
    }
}