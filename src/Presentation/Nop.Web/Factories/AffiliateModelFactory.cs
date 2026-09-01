using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Affiliate;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the class of the affiliate model factory
/// </summary>
public partial class AffiliateModelFactory : IAffiliateModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly AffiliateSettings _affiliateSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AffiliateModelFactory(AddressSettings addressSettings,
        AffiliateSettings affiliateSettings,
        CaptchaSettings captchaSettings,
        CommonSettings commonSettings,
        CurrencySettings currencySettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IDateTimeHelper dateTimeHelper,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
        _addressSettings = addressSettings;
        _affiliateSettings = affiliateSettings;
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
        _currencySettings = currencySettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _dateTimeHelper = dateTimeHelper;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _localizationService = localizationService;
        _orderService = orderService;
        _priceFormatter = priceFormatter;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
    }

    #endregion

    /// <summary>
    /// Prepares the apply affiliate model
    /// </summary>
    /// <param name="model">An apply affiliate model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the apply affiliate model
    /// </returns>
    public virtual async Task<ApplyAffiliateModel> PrepareApplyAffiliateModelAsync(ApplyAffiliateModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var affiliate = await _affiliateService.GetAffiliateByCustomerIdAsync(customer.Id);

        if (affiliate != null)
        {
            //already applied for affiliate account
            model.DisableFormInput = true;
            model.Result = await _localizationService.GetResourceAsync("Affiliate.ApplyAccount.AlreadyApplied");
        }

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyAffiliatePage;

        //existing addresses
        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is
            {
                Published: true,
                AllowsBilling: true
            } country
                &&
                //enabled for the current store
                await _storeMappingService.AuthorizeAsync(country))
            .ToListAsync();

        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);

            if (await _addressService.IsAddressValidAsync(address))
                model.ExistingAddresses.Add(addressModel);
        }

        return model;
    }

    /// <summary>
    /// Prepares the affiliate model
    /// </summary>
    /// <param name="affiliate">The affiliate</param>
    /// <param name="model">An affiliate model</param>
    /// <param name="limit">The order history period</param>
    /// <param name="page">The page number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate model
    /// </returns>
    public virtual async Task<AffiliateModel> PrepareAffiliateModelAsync(Affiliate affiliate, AffiliateModel model, OrderHistoryPeriods limit, int? page)
    {
        var periods = await Enum.GetValues<OrderHistoryPeriods>()
            .SelectAwait(async enumValue => new
            {
                ID = enumValue.ToString().ToLower(),
                Name = await _localizationService.GetLocalizedEnumAsync(enumValue)
            }).ToListAsync();

        model.AvailableLimits = new SelectList(periods, "ID", "Name", limit.ToString()).ToList();
        model.Status = await _localizationService.GetResourceAsync(affiliate.Active ? "Account.Affiliates.Status.Active" : "Account.Affiliates.Status.Disabled");
        model.TotalAffiliatedCustomers =
            (await _customerService.GetAllCustomersAsync(affiliateId: affiliate.Id, isActive: true,
                getOnlyTotalCount: true)).TotalCount;

        var pageSize = _affiliateSettings.CustomerAffiliatePageSize;
        var pageIndex = Math.Max((page ?? 0) - 1, 0);

        var orders = await _orderService.SearchOrdersAsync(
            affiliateId: affiliate.Id,
            createdFromUtc: limit == OrderHistoryPeriods.All ? null : DateTime.UtcNow.AddDays((int)limit * -1),
            createdToUtc: limit > 0 ? DateTime.UtcNow : null,
            pageIndex: pageIndex,
            pageSize: pageSize);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        model.TotalOrders = orders.TotalCount;
        model.OrdersAmount = await _priceFormatter.FormatPriceAsync(orders.Sum(o => o.OrderTotal), true, primaryStoreCurrency);
        model.OrdersCount = orders.Count;

        model.PagerModel = new PagerModel(_localizationService)
        {
            PageSize = orders.PageSize,
            TotalRecords = orders.TotalCount,
            PageIndex = orders.PageIndex,
            ShowTotalSummary = true,
            RouteActionName = NopRouteNames.Standard.CUSTOMER_AFFILIATES_INFO,
            UseRouteLinks = true,
            RouteValues =
                new CustomerAffiliateRouteValues
                {
                    PageNumber = orders.PageIndex,
                    Limit = limit.ToString().ToLower()
                }
        };

        var affiliateCommissions =
            await _affiliateService.GetAffiliateCommissionsByIdsAsync(orders.Where(o => o.AffiliateCommissionId != null).Select(o => o.AffiliateCommissionId.Value).ToArray());

        var commissions = affiliateCommissions.GroupBy(ac => ac.Id).ToDictionary(g => g.Key, g => g.FirstOrDefault());

        foreach (var order in orders)
        {
            var commissionAmount = order.AffiliateCommissionAmount.HasValue && order.AffiliateCommissionAmount > 0M ? await _priceFormatter.FormatPriceAsync(order.AffiliateCommissionAmount.Value, true, false) : string.Empty;
            var commissionStatus = string.Empty;
            DateTime? commissionPaidOn = null;

            if (order.AffiliateCommissionId.HasValue && commissions.TryGetValue(order.AffiliateCommissionId.Value, out var commission))
            {
                commissionStatus = await _localizationService.GetLocalizedEnumAsync(commission.CommissionStatus);
                commissionPaidOn = commission.PaidOn.HasValue
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(commission.PaidOn.Value, DateTimeKind.Utc)
                    : null;
            }

            var affiliateOrder = new AffiliatedOrderModel
            {
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                CustomOrderNumber = order.CustomOrderNumber,
                OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, primaryStoreCurrency),
                AffiliateCommission = commissionAmount,
                CommissionStatus = commissionStatus,
                CommissionPaidOn = commissionPaidOn
            };

            model.Orders.Add(affiliateOrder);
        }

        model.TotalCommission = await _priceFormatter.FormatPriceAsync(orders.Sum(o => o.AffiliateCommissionAmount ?? 0M), true, primaryStoreCurrency);
        var paidCommissions = 0M;

        foreach (var order in orders.Where(o=>o.AffiliateCommissionId.HasValue))
        {
            if (!commissions.TryGetValue(order.AffiliateCommissionId!.Value, out var commission))
                continue;

            if(commission.CommissionStatus != CommissionStatus.Paid)
                continue;

            paidCommissions += order.AffiliateCommissionAmount ?? 0M;
        }

        model.PaidCommission = await _priceFormatter.FormatPriceAsync(paidCommissions, true, primaryStoreCurrency);

        return model;
    }

    #region nested class

    /// <summary>
    /// Record that has filter options for route values. Used for Customer orders pagination (My Account)
    /// </summary>
    public partial record CustomerAffiliateRouteValues : BaseRouteValues
    {
        public string Limit { get; set; }
    }

    #endregion
}
