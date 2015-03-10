using System;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Seo;

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

        /// <summary>
        /// Validate friendly URL name
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="friendlyUrlName">Friendly URL name</param>
        /// <returns>Valid friendly name</returns>
        public static string ValidateFriendlyUrlName(this Affiliate affiliate, string friendlyUrlName)
        {
            //ensure we have only valid chars
            friendlyUrlName = SeoExtensions.GetSeName(friendlyUrlName);

            //max length
            //For long URLs we can get the following error:
            //"the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters"
            //that's why we limit it to 200 here (consider a store URL + probably added {0}-{1} below)
            friendlyUrlName = CommonHelper.EnsureMaximumLength(friendlyUrlName, 200);

            return friendlyUrlName;
        }
    }
}
