using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;

using Nop.Services.Localization;

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
        /// Marks affiliate as deleted 
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        void DeleteAffiliate(Affiliate affiliate);

        /// <summary>
        /// Gets all affiliates
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Affiliate collection</returns>
        IList<Affiliate> GetAllAffiliates(bool showHidden = false);

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