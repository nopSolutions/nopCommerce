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
using System.Text.RegularExpressions;
using UrlRewritingNet.Configuration;

namespace UrlRewritingNet.Web
{
    public class RegExRewriteRule : RewriteRule
    {
        private Regex regex;

        public override void Initialize(RewriteSettings rewriteSettings)
        {
            base.Initialize(rewriteSettings);
            this.RegexOptions = rewriteSettings.GetEnumAttribute<RegexOptions>("regexOptions", RegexOptions.None);
            this.VirtualUrl = rewriteSettings.GetAttribute("virtualUrl", "");
            this.destinationUrl = rewriteSettings.GetAttribute("destinationUrl", "");
        }

        private void CreateRegEx()
        {
            UrlHelper urlHelper = new UrlHelper();
            if (IgnoreCase)
            {
                this.regex = new Regex(urlHelper.HandleRootOperator(virtualUrl), RegexOptions.IgnoreCase | RegexOptions.Compiled | regexOptions);
            }
            else
            {
                this.regex = new Regex(urlHelper.HandleRootOperator(virtualUrl), RegexOptions.Compiled | regexOptions);
            }
        }

        private string virtualUrl = string.Empty;

        public string VirtualUrl
        {
            get { return virtualUrl; }
            set
            {
                virtualUrl = value;
                CreateRegEx();
            }
        }
        private string destinationUrl = string.Empty;

        public string DestinationUrl
        {
            get { return destinationUrl; }
            set { destinationUrl = value; }
        }
        private RegexOptions regexOptions = RegexOptions.None;

        public RegexOptions RegexOptions
        {
            get { return regexOptions; }
            set
            {
                regexOptions = value;
                CreateRegEx();
            }
        }

        public override bool IsRewrite(string requestUrl)
        {
            return this.regex.IsMatch(requestUrl);
        }

        public override string RewriteUrl(string url)
        {
            return this.regex.Replace(url, this.destinationUrl, 1);
        }

    }
}
