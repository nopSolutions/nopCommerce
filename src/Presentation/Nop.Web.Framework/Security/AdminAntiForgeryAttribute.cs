using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Security
{
    public class AdminAntiForgeryAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public AdminAntiForgeryAttribute(bool ignore = false) : base(typeof(AdminAntiForgeryFilter))
        {
            this.Arguments = new object[] {ignore};
        }


        #region Nested filter

        /// <summary>
        /// Represents a filter that saves last visited page by customer
        /// </summary>
        private class AdminAntiForgeryFilter : ValidateAntiforgeryTokenAuthorizationFilter
        {
            #region Fields

            private readonly bool _ignore;
            private readonly SecuritySettings _securitySettings;

            #endregion

            #region Ctor

            public AdminAntiForgeryFilter(bool ignore, SecuritySettings securitySettings, IAntiforgery antiforgery,
                ILoggerFactory loggerFactory)
                : base(antiforgery, loggerFactory)
            {
                this._ignore = ignore;
                this._securitySettings = securitySettings;
            }

            #endregion

            #region Methods

            protected override bool ShouldValidate(AuthorizationFilterContext context)
            {
                if (!base.ShouldValidate(context))
                {
                    return false;
                }

                if (_ignore)
                    return false;

                if (!_securitySettings.EnableXsrfProtectionForAdminArea)
                    return false;

                return true;
            }

            #endregion
        }

        #endregion
    }
}