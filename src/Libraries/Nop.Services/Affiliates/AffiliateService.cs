using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Affiliate service
    /// </summary>
    public partial class AffiliateService : IAffiliateService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Affiliate> _affiliateRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly SeoSettings _seoSettings;

        #endregion

        #region Ctor

        public AffiliateService(IEventPublisher eventPublisher,
            IRepository<Affiliate> affiliateRepository,
            IRepository<Order> orderRepository,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            SeoSettings seoSettings)
        {
            _eventPublisher = eventPublisher;
            _affiliateRepository = affiliateRepository;
            _orderRepository = orderRepository;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _seoSettings = seoSettings;
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
        /// Gets an affiliate by friendly URL name
        /// </summary>
        /// <param name="friendlyUrlName">Friendly URL name</param>
        /// <returns>Affiliate</returns>
        public virtual Affiliate GetAffiliateByFriendlyUrlName(string friendlyUrlName)
        {
            if (string.IsNullOrWhiteSpace(friendlyUrlName))
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
                throw new ArgumentNullException(nameof(affiliate));

            affiliate.Deleted = true;
            UpdateAffiliate(affiliate);

            //event notification
            _eventPublisher.EntityDeleted(affiliate);
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
            if (!string.IsNullOrWhiteSpace(friendlyUrlName))
                query = query.Where(a => a.FriendlyUrlName.Contains(friendlyUrlName));
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(a => a.Address.FirstName.Contains(firstName));
            if (!string.IsNullOrWhiteSpace(lastName))
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
                throw new ArgumentNullException(nameof(affiliate));

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
                throw new ArgumentNullException(nameof(affiliate));

            _affiliateRepository.Update(affiliate);

            //event notification
            _eventPublisher.EntityUpdated(affiliate);
        }

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliate full name</returns>
        public virtual string GetAffiliateFullName(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var firstName = affiliate.Address.FirstName;
            var lastName = affiliate.Address.LastName;

            var fullName = string.Empty;
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
        /// <returns>Generated affiliate URL</returns>
        public virtual string GenerateUrl(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var storeUrl = _webHelper.GetStoreLocation(false);
            var url = !string.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
                //use friendly URL
                _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateQueryParameter, affiliate.FriendlyUrlName) :
                //use ID
                _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateIdQueryParameter, affiliate.Id.ToString());

            return url;
        }

        /// <summary>
        /// Validate friendly URL name
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="friendlyUrlName">Friendly URL name</param>
        /// <returns>Valid friendly name</returns>
        public virtual string ValidateFriendlyUrlName(Affiliate affiliate, string friendlyUrlName)
        {
            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //ensure we have only valid chars
            friendlyUrlName = _urlRecordService.GetSeName(friendlyUrlName, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

            //max length
            //(consider a store URL + probably added {0}-{1} below)
            friendlyUrlName = CommonHelper.EnsureMaximumLength(friendlyUrlName, NopAffiliateDefaults.FriendlyUrlNameLength);

            //ensure this name is not reserved yet
            //empty? nothing to check
            if (string.IsNullOrEmpty(friendlyUrlName))
                return friendlyUrlName;
            //check whether such friendly URL name already exists (and that is not the current affiliate)
            var i = 2;
            var tempName = friendlyUrlName;
            while (true)
            {
                var affiliateByFriendlyUrlName = GetAffiliateByFriendlyUrlName(tempName);
                var reserved = affiliateByFriendlyUrlName != null && affiliateByFriendlyUrlName.Id != affiliate.Id;
                if (!reserved)
                    break;

                tempName = $"{friendlyUrlName}-{i}";
                i++;
            }

            friendlyUrlName = tempName;

            return friendlyUrlName;
        }

        #endregion
    }
}