using System;
using Nop.Core;
using Nop.Core.Domain.Affiliates;

namespace Nop.Services.Affiliates
{
    public static class AffiliateExtensions
    {
        /// <summary>
        /// Generate affilaite URL
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Generated affilaite URL</returns>
        public static string GenerateUrl(this Affiliate affiliate, IWebHelper webHelper)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");

            if (webHelper == null)
                throw new ArgumentNullException("webHelper");

            var storeUrl = webHelper.GetStoreLocation(false);
            var url = !String.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
                //use friendly URL
                webHelper.ModifyQueryString(storeUrl, "affiliate=" + affiliate.FriendlyUrlName, null):
                //use ID
                webHelper.ModifyQueryString(storeUrl, "affiliateid=" + affiliate.Id, null);

            return url;
        }
    }
}
