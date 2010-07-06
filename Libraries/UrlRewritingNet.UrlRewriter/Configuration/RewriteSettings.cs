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
using System.Configuration;
using System.Text.RegularExpressions;
using UrlRewritingNet.Web;
using System.Web;
using System.Security.Permissions;
using System.Collections.Specialized;

namespace UrlRewritingNet.Configuration
{
    public class RewriteSettings : ConfigurationElement
    {
        public RewriteSettings() : this((string)null)
        {

        }
        public RewriteSettings(string elementName) 
        {
            
        }

        [ConfigurationProperty("provider", IsRequired = false)]
        public string Provider
        {
            get
            {
                return (string)base["provider"];
            }
            set
            {
                base["ruleProvider"] = value;
            }
        }
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }
        [ConfigurationProperty("redirect", IsRequired = false, DefaultValue = RedirectOption.None)]
        public RedirectOption Redirect
        {
            get
            {
                return (RedirectOption)base["redirect"];
            }
            set
            {
                base["redirect"] = value;
            }
        }
        [ConfigurationProperty("rewrite", IsRequired = false, DefaultValue = RewriteOption.Application)]
        public RewriteOption Rewrite
        {
            get
            {
                return (RewriteOption)base["rewrite"];
            }
            set
            {
                base["rewrite"] = value;
            }
        }
        [ConfigurationProperty("redirectMode", IsRequired = false, DefaultValue = RedirectModeOption.Temporary)]
        public RedirectModeOption RedirectMode
        {
            get
            {
                return (RedirectModeOption)base["redirectMode"];
            }
            set
            {
                base["redirectMode"] = value;
            }
        }

        [ConfigurationProperty("rewriteUrlParameter", DefaultValue = RewriteUrlParameterOption.ExcludeFromClientQueryString)]
        public RewriteUrlParameterOption RewriteUrlParameter
        {
            get
            {
                return (RewriteUrlParameterOption)base["rewriteUrlParameter"];
            }
            set
            {
                base["rewriteUrlParameter"] = value;
            }
        }

        [ConfigurationProperty("ignoreCase", IsRequired = false, DefaultValue = false)]
        public bool IgnoreCase
        {
            get
            {
                return (bool)base["ignoreCase"];
            }
            set
            {
                base["ignoreCase"] = value;
            }
        }

        private NameValueCollection attributes = new NameValueCollection();

        public NameValueCollection Attributes
        {
            get
            {
                return attributes;
            }
        }

        public string GetAttribute(string name, string defaultValue)
        {
            if (attributes[name] == null)
                return defaultValue;
            else
                return attributes[name];
        }

        public int GetInt32Attribute(string name, int defaultValue)
        {
            if (attributes[name] == null)
                return defaultValue;
            else
                return Convert.ToInt32(attributes[name]);
        }
        public bool GetBooleanAttribute(string name, bool defaultValue)
        {
            if (attributes[name] == null)
                return defaultValue;
            else
                return bool.Parse(attributes[name]);
        }
        public T GetEnumAttribute<T>(string name, T defaultValue)
        {
            if (attributes[name] == null)
                return defaultValue;
            else
                return (T) Enum.Parse(typeof(T), attributes[name]);
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            attributes.Add(name, value);
            return true;
        }

    }
}
