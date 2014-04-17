using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class NopHttpsRequirementAttribute : FilterAttribute, IAuthorizationFilter
    {
        public NopHttpsRequirementAttribute(SslRequirement sslRequirement)
        {
            this.SslRequirement = sslRequirement;
        }
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;
            
            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;
            
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            if (securitySettings.ForceSslForAllPages)
                //all pages are forced to be SSL no matter of the specified value
                this.SslRequirement = SslRequirement.Yes;
            
            switch (this.SslRequirement)
            {
                case SslRequirement.Yes:
                    {
                        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        var currentConnectionSecured = webHelper.IsCurrentConnectionSecured();
                        if (!currentConnectionSecured)
                        {
                            var storeContext = EngineContext.Current.Resolve<IStoreContext>();
                            if (storeContext.CurrentStore.SslEnabled)
                            {
                                //redirect to HTTPS version of page
                                //string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                                string url = webHelper.GetThisPageUrl(true, true);

                                //301 (permanent) redirection
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                    }
                    break;
                case SslRequirement.No:
                    {
                        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        var currentConnectionSecured = webHelper.IsCurrentConnectionSecured();
                        if (currentConnectionSecured)
                        {
                            //redirect to HTTP version of page
                            //string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                            string url = webHelper.GetThisPageUrl(true, false);
                            //301 (permanent) redirection
                            filterContext.Result = new RedirectResult(url, true);
                        }
                    }
                    break;
                case SslRequirement.NoMatter:
                    {
                        //do nothing
                    }
                    break;
                default:
                    throw new NopException("Not supported SslProtected parameter");
            }
        }

        public SslRequirement SslRequirement { get; set; }
    }
}
