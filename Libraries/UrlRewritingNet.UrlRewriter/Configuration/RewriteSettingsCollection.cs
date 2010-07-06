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
using System.Web;
using System.Security.Permissions;
using UrlRewritingNet.Web;
using System.Xml;

namespace UrlRewritingNet.Configuration
{
    [ConfigurationCollection(typeof(RewriteSettings))]
    public sealed class RewriteSettingsCollection : ConfigurationElementCollection
    {
        private string currentRuleProvider = String.Empty;

        protected override ConfigurationElement CreateNewElement() 
        {
            return new RewriteSettings();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RewriteSettings)element).Name;
        }

        public RewriteSettings this[int index]
        {
            get
            {
                return (RewriteSettings)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }


        new public RewriteSettings this[string key]
        {
            get
            {
                return (RewriteSettings)base.BaseGet(key);
            }
        }

    }
}
