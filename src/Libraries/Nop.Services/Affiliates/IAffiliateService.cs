using Nop.Core;
using Nop.Core.Domain.Affiliates;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Affiliate service interface
    /// </summary>
    public partial interface IAffiliateService
    {
        /// <summary>
        /// Gets an affiliate by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Affiliate</returns>
        Affiliate GetAffiliateById(int affiliateId);

        /// <summary>
        /// Gets an affiliate by friendly url name
        /// </summary>
        /// <param name="friendlyUrlName">Friendly url name</param>
        /// <returns>Affiliate</returns>
        Affiliate GetAffiliateByFriendlyUrlName(string friendlyUrlName);

        /// <summary>
        /// Marks affiliate as deleted 
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        void DeleteAffiliate(Affiliate affiliate);

        /// <summary>
        /// Gets all affiliates
        /// </summary>
        /// <param name="friendlyUrlName">Friendly URL name; null to load all records</param>
        /// <param name="firstName">First name; null to load all records</param>
        /// <param name="lastName">Last name; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Affiliates</returns>
        IPagedList<Affiliate> GetAllAffiliates(string friendlyUrlName = null, 
            string firstName = null, string lastName = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = false);

        /// <summary>
        /// Inserts an affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        void InsertAffiliate(Affiliate affiliate);

        /// <summary>
        /// Updates the affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        void UpdateAffiliate(Affiliate affiliate);
        
    }
}