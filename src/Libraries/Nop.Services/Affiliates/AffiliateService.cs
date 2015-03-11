using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Affiliate service
    /// </summary>
    public partial class AffiliateService : IAffiliateService
    {
        #region Fields

        private readonly IRepository<Affiliate> _affiliateRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="affiliateRepository">Affiliate repository</param>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="eventPublisher">Event published</param>
        public AffiliateService(IRepository<Affiliate> affiliateRepository,
            IRepository<Order> orderRepository,
            IEventPublisher eventPublisher)
        {
            this._affiliateRepository = affiliateRepository;
            this._orderRepository = orderRepository;
            this._eventPublisher = eventPublisher;
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
        /// Gets an affiliate by friendly url name
        /// </summary>
        /// <param name="friendlyUrlName">Friendly url name</param>
        /// <returns>Affiliate</returns>
        public virtual Affiliate GetAffiliateByFriendlyUrlName(string friendlyUrlName)
        {
            if (String.IsNullOrWhiteSpace(friendlyUrlName))
                return null;

            var query = from a in _affiliateRepository.Table
                        orderby a.Id
                        where a.FriendlyUrlName == friendlyUrlName
                        select a;
            var affiliate = query.FirstOrDefault();
            return affiliate;
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
        /// <param name="friendlyUrlName">Friendly URL name; null to load all records</param>
        /// <param name="firstName">First name; null to load all records</param>
        /// <param name="lastName">Last name; null to load all records</param>
        /// <param name="loadOnlyWithOrders">Value indicating whether to load affiliates only with orders placed (by affiliated customers)</param>
        /// <param name="ordersCreatedFromUtc">Orders created date from (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
        /// <param name="ordersCreatedToUtc">Orders created date to (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Affiliates</returns>
        public virtual IPagedList<Affiliate> GetAllAffiliates(string friendlyUrlName = null,
            string firstName = null, string lastName = null,
            bool loadOnlyWithOrders = false,
            DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            var query = _affiliateRepository.Table;
            if (!String.IsNullOrWhiteSpace(friendlyUrlName))
                query = query.Where(a => a.FriendlyUrlName.Contains(friendlyUrlName));
            if (!String.IsNullOrWhiteSpace(firstName))
                query = query.Where(a => a.Address.FirstName.Contains(firstName));
            if (!String.IsNullOrWhiteSpace(lastName))
                query = query.Where(a => a.Address.LastName.Contains(lastName));
            if (!showHidden)
                query = query.Where(a => a.Active);
            query = query.Where(a => !a.Deleted);

            if (loadOnlyWithOrders)
            {
                var ordersQuery = _orderRepository.Table;
                if (ordersCreatedFromUtc.HasValue)
                    ordersQuery = ordersQuery.Where(o => ordersCreatedFromUtc.Value <= o.CreatedOnUtc);
                if (ordersCreatedToUtc.HasValue)
                    ordersQuery = ordersQuery.Where(o => ordersCreatedToUtc.Value >= o.CreatedOnUtc);
                ordersQuery = ordersQuery.Where(o => !o.Deleted);

                query = from a in query
                        join o in ordersQuery on a.Id equals o.AffiliateId into a_o
                        where a_o.Any()
                        select a;
            }

            query = query.OrderByDescending(a => a.Id);

            var affiliates = new PagedList<Affiliate>(query, pageIndex, pageSize);
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

            //event notification
            _eventPublisher.EntityInserted(affiliate);
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

            //event notification
            _eventPublisher.EntityUpdated(affiliate);
        }

        #endregion
    }
}