/* UrlRewritingNet.UrlRewrite
 * Version 2.0
 * 
 * This Library is Copyright 2006 by Albert Weinert and Thomas Bandt.
 * 
 * http://der-albert.com, http://blog.thomasbandt.de
 * 
 * This Library is provided as is. No warrenty is expressed or implied.
 * 
 * You can use these Library in free and commercial projects without a fee.
 * 
 * No charge should be made for providing these Library to a third party.
 * 
 * It is allowed to modify the source to fit your special needs. If you 
 * made improvements you should make it public available by sending us 
 * your modifications or publish it on your site. If you publish it on 
 * your own site you have to notify us. This is not a commitment that we 
 * include your modifications. 
 * 
 * This Copyright notice must be included in the modified source code.
 * 
 * You are not allowed to build a commercial rewrite engine based on 
 * this code.
 * 
 * Based on http://weblogs.asp.net/fmarguerie/archive/2004/11/18/265719.aspx
 * 
 * For further informations see: http://www.urlrewriting.net/
 */

using System;
using System.Collections.Generic;
using System.Text;
using UrlRewritingNet.Configuration.Provider;
using System.Web.Configuration;
using UrlRewritingNet.Configuration;
using System.Configuration;
using System.Web;
using System.Collections.Specialized;

namespace UrlRewritingNet.Web
{
    static public class UrlRewriting
    {
        static UrlRewritingProviderCollection providers = null;

        static bool initialized = false;

        static private void Initialize()
        {
            if (!initialized)
            {
                providers = new UrlRewritingProviderCollection();
                ProvidersHelper.InstantiateProviders(Configuration.Providers, providers, typeof(UrlRewritingProvider));
                if (providers["RegEx"] == null)
                {
                    RegExUrlRewritingProvider prov = new RegExUrlRewritingProvider();
                    prov.Initialize("RegEx", new NameValueCollection());
                    providers.Add(prov);
                }

                if (providers[Configuration.DefaultProvider] == null)
                {
                    string msg = string.Format("Missing the DefaultProvider {0} in the list of providers for UrlRewritingNet", Configuration.DefaultProvider);
                    throw new ApplicationException(msg);
                }
                initialized = true;
            }
        }

        static private UrlRewriteSection configuration;
        static private object configurationLock = new object();

        static public UrlRewriteSection Configuration
        {
            get
            {
                if (configuration == null)
                {
                    lock (configurationLock)
                    {
                        if (configuration == null)
                        {
                            UrlRewriteSection tmpConf = (UrlRewriteSection)ConfigurationManager.GetSection("urlrewritingnet");
                            configuration = tmpConf;
                        }
                    }
                }
                return configuration;
            }
        }

        static public UrlRewritingProviderCollection Providers
        {
            get
            {
                Initialize();
                return providers;
            }
        }

        static public RewriteRule CreateRewriteRule()
        {
            return CreateRewriteRule(Configuration.DefaultProvider);
        }
        public static RewriteRule CreateRewriteRule(string providerName)
        {
            if (providerName == null)
                throw new ArgumentNullException("providerName");

            UrlRewritingProvider provider = Providers[providerName];
            if (provider == null)
            {
                string msg = string.Format("Unknown UrlRewritingProvider {0} in list of rules", providerName);
                throw new ArgumentException(msg, "providerName");
            }
            return provider.CreateRewriteRule();
        }

        public static void AddRewriteRule(string ruleName, RewriteRule rewriteRule)
        {
            if (rewriteRule == null)
                throw new ArgumentNullException("rewriteRule");
            rewriteRule.Name = ruleName;
            HttpModuleCollection modules = System.Web.HttpContext.Current.ApplicationInstance.Modules;
            foreach (string moduleName in modules)
            {
                UrlRewriteModule rewriteModule = modules[moduleName] as UrlRewriteModule;
                if (rewriteModule != null)
                {
                    rewriteModule.AddRewriteRuleInternal(rewriteRule);
                }
            }
        }

        public static void RemoveRewriteRule(string ruleName)
        {
            HttpModuleCollection modules = System.Web.HttpContext.Current.ApplicationInstance.Modules;
            foreach (string moduleName in modules)
            {
                UrlRewriteModule rewriteModule = modules[moduleName] as UrlRewriteModule;
                if (rewriteModule != null)
                {
                    rewriteModule.RemoveRewriteRuleInternal(ruleName);
                }
            }
        }

        public static void ReplaceRewriteRule(string ruleName, RewriteRule rewriteRule)
        {
            if (rewriteRule == null)
                throw new ArgumentNullException("rewriteRule");
            rewriteRule.Name = ruleName;
            HttpModuleCollection modules = System.Web.HttpContext.Current.ApplicationInstance.Modules;
            foreach (string moduleName in modules)
            {
                UrlRewriteModule rewriteModule = modules[moduleName] as UrlRewriteModule;
                if (rewriteModule != null)
                {
                    rewriteModule.ReplaceRewriteRuleInternal(ruleName, rewriteRule);
                }
            }
        }

        public static void InsertRewriteRule(string positionRuleName, string ruleName, RewriteRule rewriteRule)
        {
            if (rewriteRule == null)
                throw new ArgumentNullException("rewriteRule");
            rewriteRule.Name = ruleName;
            HttpModuleCollection modules = System.Web.HttpContext.Current.ApplicationInstance.Modules;
            foreach (string moduleName in modules)
            {
                UrlRewriteModule rewriteModule = modules[moduleName] as UrlRewriteModule;
                if (rewriteModule != null)
                {
                    rewriteModule.InsertRewriteRuleBeforeInternal( positionRuleName, ruleName, rewriteRule);
                }
            }
        }

    }
}
