using System;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Infrastructure;
using Nop.Services.Seo;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Affiliate extensions
    /// </summary>
    public static class AffiliateExtensions
    {
        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliate full name</returns>
        public static string GetFullName(this Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var firstName = affiliate.Address.FirstName;
            var lastName = affiliate.Address.LastName;

            var fullName = "";
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }

        /// <summary>
        /// Generate affiliate URL
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Generated affiliate URL</returns>
        public static string GenerateUrl(this Affiliate affiliate, IWebHelper webHelper)
        {
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            if (webHelper == null)
                throw new ArgumentNullException(nameof(webHelper));

            var storeUrl = webHelper.GetStoreLocation(false);
            var url = !string.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
                //use friendly URL
                webHelper.ModifyQueryString(storeUrl, "affiliate=" + affiliate.FriendlyUrlName, null) :
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
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //ensure we have only valid chars
            friendlyUrlName = SeoExtensions.GetSeName(friendlyUrlName);

            //max length
            //For long URLs we can get the following error:
            //"the specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters"
            //that's why we limit it to 200 here (consider a store URL + probably added {0}-{1} below)
            friendlyUrlName = CommonHelper.EnsureMaximumLength(friendlyUrlName, 200);

            //ensure this name is not reserved yet
            //empty? nothing to check
            if (string.IsNullOrEmpty(friendlyUrlName))
                return friendlyUrlName;
            //check whether such friendly URL name already exists (and that is not the current affiliate)
            var i = 2;
            var tempName = friendlyUrlName;
            while (true)
            {
                var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                var affiliateByFriendlyUrlName = affiliateService.GetAffiliateByFriendlyUrlName(tempName);

                var reserved = affiliateByFriendlyUrlName != null && affiliateByFriendlyUrlName.Id != affiliate.Id;
                if (!reserved)
                    break;

                tempName = $"{friendlyUrlName}-{i}";
                i++;
            }
            friendlyUrlName = tempName;

            return friendlyUrlName;
        }
    }
}
