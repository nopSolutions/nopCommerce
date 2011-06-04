using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;

using Nop.Services.Localization;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Affiliate service
    /// </summary>
    public partial class AffiliateService : IAffiliateService
    {
        #region Fields

        private readonly IRepository<Affiliate> _affiliateRepository;
        
        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="affiliateRepository">Affiliate repository</param>
        public AffiliateService(IRepository<Affiliate> affiliateRepository)
        {
            this._affiliateRepository = affiliateRepository;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets an affiliate by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Affiliate</returns>
        public virtual Affiliate GetAffiliateById(int affiliateId)
        {
            if (affiliateId == 0)
                return null;
            
            return _affiliateRepository.GetById(affiliateId);
        }

        /// <summary>
        /// Marks affiliate as deleted 
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public virtual void DeleteAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");

            affiliate.Deleted = true;
            UpdateAffiliate(affiliate);
        }

        /// <summary>
        /// Gets all affiliates
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Affiliate collection</returns>
        public virtual IList<Affiliate> GetAllAffiliates(bool showHidden = false)
        {
            var query = from a in _affiliateRepository.Table
                        orderby a.Id
                        where (showHidden || a.Active) && !a.Deleted
                        select a;
            var affiliates = query.ToList();

            return affiliates;
        }

        /// <summary>
        /// Inserts an affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public virtual void InsertAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");

            _affiliateRepository.Insert(affiliate);
        }

        /// <summary>
        /// Updates the affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public virtual void UpdateAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");

            _affiliateRepository.Update(affiliate);
        }

        #endregion
        
    }
}