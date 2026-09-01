using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.Seo;

namespace Nop.Services.Affiliates;

/// <summary>
/// Affiliate service
/// </summary>
public partial class AffiliateService : IAffiliateService
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IRepository<Address> _addressRepository;
    protected readonly IRepository<Affiliate> _affiliateRepository;
    protected readonly IRepository<AffiliateCommission> _affiliateCommissionRepository;
    protected readonly IRepository<Order> _orderRepository;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public AffiliateService(IAddressService addressService,
        IRepository<Address> addressRepository,
        IRepository<Affiliate> affiliateRepository,
        IRepository<AffiliateCommission> affiliateCommissionRepository,
        IRepository<Order> orderRepository,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        SeoSettings seoSettings)
    {
        _addressService = addressService;
        _addressRepository = addressRepository;
        _affiliateRepository = affiliateRepository;
        _affiliateCommissionRepository = affiliateCommissionRepository;
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    public virtual async Task<Affiliate> GetAffiliateByIdAsync(int affiliateId)
    {
        return await _affiliateRepository.GetByIdAsync(affiliateId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets an affiliate by customer identifier
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    public virtual async Task<Affiliate> GetAffiliateByCustomerIdAsync(int customerId)
    {
        return await _affiliateRepository.Table.FirstOrDefaultAsync(a=>a.AssociatedCustomerId == customerId);
    }

    /// <summary>
    /// Gets an affiliate by friendly URL name
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    public virtual async Task<Affiliate> GetAffiliateByFriendlyUrlNameAsync(string friendlyUrlName)
    {
        if (string.IsNullOrWhiteSpace(friendlyUrlName))
            return null;

        var query = from a in _affiliateRepository.Table
            orderby a.Id
            where a.FriendlyUrlName == friendlyUrlName
            select a;
        var affiliate = await query.FirstOrDefaultAsync();

        return affiliate;
    }

    /// <summary>
    /// Marks affiliate as deleted 
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAffiliateAsync(Affiliate affiliate)
    {
        await _affiliateRepository.DeleteAsync(affiliate);
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliates
    /// </returns>
    public virtual async Task<IPagedList<Affiliate>> GetAllAffiliatesAsync(string friendlyUrlName = null,
        string firstName = null, string lastName = null,
        bool loadOnlyWithOrders = false,
        DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue,
        bool showHidden = false)
    {
        return await _affiliateRepository.GetAllPagedAsync(query =>
        {
            if (!string.IsNullOrWhiteSpace(friendlyUrlName))
                query = query.Where(a => a.FriendlyUrlName.Contains(friendlyUrlName));

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.FirstName.Contains(firstName)
                    select aff;
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.LastName.Contains(lastName)
                    select aff;
            }

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
                    join o in ordersQuery on a.Id equals o.AffiliateId
                    select a;
            }

            query = query.Distinct().OrderByDescending(a => a.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Inserts an affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAffiliateAsync(Affiliate affiliate)
    {
        await _affiliateRepository.InsertAsync(affiliate);
    }

    /// <summary>
    /// Updates the affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAffiliateAsync(Affiliate affiliate)
    {
        await _affiliateRepository.UpdateAsync(affiliate);
    }

    /// <summary>
    /// Get full name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate full name
    /// </returns>
    public virtual async Task<string> GetAffiliateFullNameAsync(Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(affiliate);

        var affiliateAddress = await _addressService.GetAddressByIdAsync(affiliate.AddressId);

        if (affiliateAddress == null)
            return string.Empty;

        var fullName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

        return fullName;
    }

    /// <summary>
    /// Generate affiliate URL
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated affiliate URL
    /// </returns>
    public virtual Task<string> GenerateUrlAsync(Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(affiliate);

        var storeUrl = _webHelper.GetStoreLocation();
        var url = !string.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
            //use friendly URL
            _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateQueryParameter, affiliate.FriendlyUrlName) :
            //use ID
            _webHelper.ModifyQueryString(storeUrl, NopAffiliateDefaults.AffiliateIdQueryParameter, affiliate.Id.ToString());

        return Task.FromResult(url);
    }

    /// <summary>
    /// Validate friendly URL name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid friendly name
    /// </returns>
    public virtual async Task<string> ValidateFriendlyUrlNameAsync(Affiliate affiliate, string friendlyUrlName)
    {
        ArgumentNullException.ThrowIfNull(affiliate);

        //ensure we have only valid chars
        friendlyUrlName = await _urlRecordService.GetSeNameAsync(friendlyUrlName, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

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
            var affiliateByFriendlyUrlName = await GetAffiliateByFriendlyUrlNameAsync(tempName);
            var reserved = affiliateByFriendlyUrlName != null && affiliateByFriendlyUrlName.Id != affiliate.Id;
            if (!reserved)
                break;

            tempName = $"{friendlyUrlName}-{i}";
            i++;
        }

        friendlyUrlName = tempName;

        return friendlyUrlName;
    }

    /// <summary>
    /// Inserts an affiliate commission
    /// </summary>
    /// <param name="commission">Affiliate commission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAffiliateCommission(AffiliateCommission commission)
    {
        await _affiliateCommissionRepository.InsertAsync(commission);
    }

    /// <summary>
    /// Gets the affiliate commission by order identifiers
    /// </summary>
    /// <param name="commissionIds">The list of affiliate commission identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of affiliate commissions
    /// </returns>
    public virtual async Task<IList<AffiliateCommission>> GetAffiliateCommissionsByIdsAsync(int[] commissionIds)
    {
        if (!commissionIds.Any())
            return new List<AffiliateCommission>();

        return await _affiliateCommissionRepository.Table.Where(ac => commissionIds.Contains(ac.Id)).ToListAsync();
    }

    /// <summary>
    /// Gets an affiliate commission by affiliate identifier
    /// </summary>
    /// <param name="commissionId">Affiliate commission identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission
    /// </returns>
    public virtual async Task<AffiliateCommission> GetAffiliateCommissionByIdAsync(int commissionId)
    {
        return await _affiliateCommissionRepository.GetByIdAsync(commissionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Updates the affiliate commission
    /// </summary>
    /// <param name="commission">Affiliate commission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAffiliateCommissionAsync(AffiliateCommission commission)
    {
        await _affiliateCommissionRepository.UpdateAsync(commission);
    }
    
    /// <summary>
    /// Gets all affiliate commissions
    /// </summary>
    /// <param name="affiliateId">Affiliate identifier</param>
    /// <param name="createdFromUtc">Created from UTC</param>
    /// <param name="createdToUtc">Created to UTC</param>
    /// <param name="commissionStatus">Commission status identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commissions
    /// </returns>
    public virtual async Task<IPagedList<AffiliateCommission>> GetAllCommissionsAsync(int affiliateId,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int? commissionStatus = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false)
    {
        var query = _affiliateCommissionRepository.Table.Where(ac=>ac.AffiliateId == affiliateId);

        if (createdFromUtc.HasValue)
            query = query.Where(c => c.CreateOn >= createdFromUtc.Value);

        if (createdToUtc.HasValue)
            query = query.Where(c => c.CreateOn <= createdToUtc.Value);

        if (commissionStatus.HasValue)
            query = query.Where(c => c.CommissionStatusId == commissionStatus.Value);

        return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
    }


    /// <summary>
    /// Delete affiliate commissions
    /// </summary>
    /// <param name="commission">Commission to delete</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteAffiliateCommissionAsync(AffiliateCommission commission)
    {
        var orders = await _orderRepository.Table.Where(o => o.AffiliateCommissionId == commission.Id).ToListAsync();
        
        foreach (var order in orders) 
            order.AffiliateCommissionId = null;

        await _orderRepository.UpdateAsync(orders);

        await _affiliateCommissionRepository.DeleteAsync(commission);
    }

    #endregion
}