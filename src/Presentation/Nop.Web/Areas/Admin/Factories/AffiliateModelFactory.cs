using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the affiliate model factory implementation
/// </summary>
public partial class AffiliateModelFactory : IAffiliateModelFactory
{
    #region Fields

    protected readonly AffiliateSettings _affiliateSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IStateProvinceService _stateProvinceService;

    #endregion

    #region Ctor

    public AffiliateModelFactory(AffiliateSettings affiliateSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IStateProvinceService stateProvinceService)
    {
        _affiliateSettings = affiliateSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _orderService = orderService;
        _priceFormatter = priceFormatter;
        _stateProvinceService = stateProvinceService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare affiliated order search model
    /// </summary>
    /// <param name="searchModel">Affiliated order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated order search model
    /// </returns>
    protected virtual async Task<AffiliatedOrderSearchModel> PrepareAffiliatedOrderSearchModelAsync(AffiliatedOrderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare affiliated customer search model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>Affiliated customer search model</returns>
    protected virtual AffiliatedCustomerSearchModel PrepareAffiliatedCustomerSearchModel(AffiliatedCustomerSearchModel searchModel, Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        searchModel.AffliateId = affiliate.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare affiliated customer search model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>Affiliated customer search model</returns>
    protected virtual async Task<AffiliateCommissionSearchModel> PrepareAffiliateCommissionSearchModelAsync(AffiliateCommissionSearchModel searchModel, Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        searchModel.AffliateId = affiliate.Id;

        //prepare available order statuses
        var availableStatusItems = await CommissionStatus.Pending.ToSelectListAsync(false);
        foreach (var statusItem in availableStatusItems)
            searchModel.AvailableCommissionStatus.Add(statusItem);

        //insert this default item at first
        searchModel.AvailableCommissionStatus.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare affiliate customer search model
    /// </summary>
    /// <param name="searchModel">Affiliate customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate customer search model
    /// </returns>
    public virtual Task<AffiliateCustomerSearchModel> PrepareAffiliateCustomerSearchModelAsync(AffiliateCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare affiliate search model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate search model
    /// </returns>
    public virtual Task<AffiliateSearchModel> PrepareAffiliateSearchModelAsync(AffiliateSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged affiliate list model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate list model
    /// </returns>
    public virtual async Task<AffiliateListModel> PrepareAffiliateListModelAsync(AffiliateSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get affiliates
        var affiliates = await _affiliateService.GetAllAffiliatesAsync(searchModel.SearchFriendlyUrlName,
            searchModel.SearchFirstName,
            searchModel.SearchLastName,
            searchModel.LoadOnlyWithOrders,
            searchModel.OrdersCreatedFromUtc,
            searchModel.OrdersCreatedToUtc,
            searchModel.Page - 1, searchModel.PageSize, true);

        //prepare list model
        var model = await new AffiliateListModel().PrepareToGridAsync(searchModel, affiliates, () =>
        {
            //fill in model values from the entity
            return affiliates.SelectAwait(async affiliate =>
            {
                var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);

                var affiliateModel = affiliate.ToModel<AffiliateModel>();
                affiliateModel.Address = address.ToModel<AddressModel>();
                affiliateModel.Address.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                affiliateModel.Address.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                return affiliateModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare affiliate model
    /// </summary>
    /// <param name="model">Affiliate model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate model
    /// </returns>
    public virtual async Task<AffiliateModel> PrepareAffiliateModelAsync(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
    {
        //fill in model values from the entity
        if (affiliate != null)
        {
            model ??= affiliate.ToModel<AffiliateModel>();
            model.Url = await _affiliateService.GenerateUrlAsync(affiliate);

            model.AffiliatedOrderSearchModel.AffiliateId = affiliate.Id;

            //prepare nested search models
            await PrepareAffiliatedOrderSearchModelAsync(model.AffiliatedOrderSearchModel);
            PrepareAffiliatedCustomerSearchModel(model.AffiliatedCustomerSearchModel, affiliate);
            await PrepareAffiliateCommissionSearchModelAsync(model.AffiliateCommissionSearchModel, affiliate);

            if (affiliate.AssociatedCustomerId > 0)
            {
                var associatedCustomer = await _customerService.GetCustomerByIdAsync(affiliate.AssociatedCustomerId.Value);
                model.CustomerInfo = await _customerService.FormatUsernameAsync(associatedCustomer) + $" {associatedCustomer.Email}";
            }

            //whether to fill in some of properties
            if (!excludeProperties)
            {
                model.AdminComment = affiliate.AdminComment;
                model.FriendlyUrlName = affiliate.FriendlyUrlName;
                model.Active = affiliate.Active;
            }
        }

        //prepare address model
        var address = await _addressService.GetAddressByIdAsync(affiliate?.AddressId ?? 0);
        if (!excludeProperties && address != null)
            model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.FirstNameRequired = true;
        model.Address.LastNameRequired = true;
        model.Address.EmailRequired = true;
        model.Address.CountryRequired = true;
        model.Address.CountyRequired = true;
        model.Address.CityRequired = true;
        model.Address.StreetAddressRequired = true;
        model.Address.ZipPostalCodeRequired = true;
        model.Address.PhoneRequired = true;

        return model;
    }

    /// <summary>
    /// Prepare paged affiliated order list model
    /// </summary>
    /// <param name="searchModel">Affiliated order search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated order list model
    /// </returns>
    public virtual async Task<AffiliatedOrderListModel> PrepareAffiliatedOrderListModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        //get parameters to filter orders
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var orderStatusIds = searchModel.OrderStatusId > 0 ? new List<int> { searchModel.OrderStatusId } : null;
        var paymentStatusIds = searchModel.PaymentStatusId > 0 ? new List<int> { searchModel.PaymentStatusId } : null;
        var shippingStatusIds = searchModel.ShippingStatusId > 0 ? new List<int> { searchModel.ShippingStatusId } : null;

        int? commissionId = searchModel.IsCommissionPage ? (searchModel.AffiliateCommissionId == 0 ? null : searchModel.AffiliateCommissionId) : 0;

        //get orders
        var orders = await _orderService.SearchOrdersAsync(createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            affiliateId: affiliate.Id,
            affiliateCommissionId: commissionId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var affiliateCommissions =
            await _affiliateService.GetAffiliateCommissionsByIdsAsync(orders.Where(o=>o.AffiliateCommissionId != null).Select(o => o.AffiliateCommissionId.Value).ToArray());

        var commissions = affiliateCommissions.GroupBy(ac => ac.Id).ToDictionary(g => g.Key, g => g.FirstOrDefault());

        //prepare list model
        var model = await new AffiliatedOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            //fill in model values from the entity
            return orders.SelectAwait(async order =>
            {
                var affiliatedOrderModel = order.ToModel<AffiliatedOrderModel>();

                //fill in additional values (not existing in the entity)
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

                affiliatedOrderModel.AffiliateCommission = commissionAmount;
                affiliatedOrderModel.CommissionStatus = commissionStatus;
                affiliatedOrderModel.PaidOn = commissionPaidOn;


                affiliatedOrderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                affiliatedOrderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                affiliatedOrderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                affiliatedOrderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                affiliatedOrderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                return affiliatedOrderModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare affiliate commission list model
    /// </summary>
    /// <param name="searchModel">Affiliated commission search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission list model
    /// </returns>
    public virtual async Task<AffiliateCommissionListModel> PrepareAffiliateCommissionListModelAsync(
        AffiliateCommissionSearchModel searchModel, Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(affiliate);

        //get parameters to filter orders
        var startDateValue = !searchModel.StartDate.HasValue
            ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value,
                await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue
            ? null
            : (DateTime?)_dateTimeHelper
                .ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
                .AddDays(1);

        int? commissionStatusId = searchModel.CommissionStatusId > 0 ? searchModel.CommissionStatusId : null;

        var commission = await _affiliateService.GetAllCommissionsAsync(searchModel.AffliateId, startDateValue,
            endDateValue, commissionStatusId, searchModel.Page - 1, searchModel.PageSize);

        //prepare list model
        var model = await new AffiliateCommissionListModel().PrepareToGridAsync(searchModel, commission, () =>
        {
            //fill in model values from the entity
            return commission.SelectAwait(async affiliateCommission =>
            {
                var affiliateCommissionModel = affiliateCommission.ToModel<AffiliateCommissionModel>();

                //fill in additional values (not existing in the entity)
                var commissionAmount =
                    await _priceFormatter.FormatPriceAsync(affiliateCommission.TotalCommissionAmount, true, false);
                var commissionStatus =
                    await _localizationService.GetLocalizedEnumAsync(affiliateCommission.CommissionStatus);
                DateTime? commissionPaidOn = affiliateCommission.PaidOn.HasValue
                    ? await _dateTimeHelper.ConvertToUserTimeAsync(affiliateCommission.PaidOn.Value, DateTimeKind.Utc)
                    : null;
                
                affiliateCommissionModel.TotalCommissionAmount = commissionAmount;
                affiliateCommissionModel.CommissionStatus = commissionStatus;
                affiliateCommissionModel.PaidOn = commissionPaidOn;

                affiliateCommissionModel.CreateOn =
                    await _dateTimeHelper.ConvertToUserTimeAsync(affiliateCommission.CreateOn, DateTimeKind.Utc);

                return affiliateCommissionModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged affiliated customer list model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated customer list model
    /// </returns>
    public virtual async Task<AffiliatedCustomerListModel> PrepareAffiliatedCustomerListModelAsync(AffiliatedCustomerSearchModel searchModel,
        Affiliate affiliate)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(affiliate);

        //get customers
        var customers = await _customerService.GetAllCustomersAsync(affiliateId: affiliate.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new AffiliatedCustomerListModel().PrepareToGrid(searchModel, customers, () =>
        {
            //fill in model values from the entity
            return customers.Select(customer =>
            {
                var affiliatedCustomerModel = customer.ToModel<AffiliatedCustomerModel>();
                affiliatedCustomerModel.Name = customer.Email;

                return affiliatedCustomerModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged affiliate customer list model
    /// </summary>
    /// <param name="searchModel">Affiliate customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate customer list model
    /// </returns>
    public virtual async Task<AffiliateCustomerListModel> PrepareAffiliateCustomerListModelAsync(AffiliateCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customers
        var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
        var customers = await _customerService.GetAllCustomersAsync(
            email: searchModel.SearchEmail,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            company: searchModel.SearchCompany,
            customerRoleIds: searchCustomerRoleIds,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AffiliateCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer => new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Company = customer.Company,
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare affiliate commission editor model
    /// </summary>
    /// <param name="affiliateCommissionEditModel">Affiliate commission editor model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission editor model
    /// </returns>
    public virtual async Task<AffiliateCommissionEditModel> PrepareAffiliateCommissionEditModelAsync(
        AffiliateCommissionEditModel affiliateCommissionEditModel)
    {
        ArgumentNullException.ThrowIfNull(affiliateCommissionEditModel);
        affiliateCommissionEditModel.AffiliatedOrderSearchModel.AffiliateId = affiliateCommissionEditModel.AffiliateId;

        await PrepareAffiliatedOrderSearchModelAsync(affiliateCommissionEditModel.AffiliatedOrderSearchModel);

        var affiliateCommission = await _affiliateService.GetAffiliateCommissionByIdAsync(affiliateCommissionEditModel.Id);

        if (affiliateCommission != null)
        {
            affiliateCommissionEditModel.AdminComment = affiliateCommission.AdminComment;

            affiliateCommissionEditModel.TotalCommissionAmount =
                await _priceFormatter.FormatPriceAsync(affiliateCommission.TotalCommissionAmount, true, false);

            affiliateCommissionEditModel.CreateOn =
                await _dateTimeHelper.ConvertToUserTimeAsync(affiliateCommission.CreateOn, DateTimeKind.Utc);
                
            var commissionStatus =
                await _localizationService.GetLocalizedEnumAsync(affiliateCommission.CommissionStatus);
            DateTime? commissionPaidOn = affiliateCommission.PaidOn.HasValue
                ? await _dateTimeHelper.ConvertToUserTimeAsync(affiliateCommission.PaidOn.Value, DateTimeKind.Utc)
                : null;

            affiliateCommissionEditModel.CommissionStatus = commissionStatus;
            affiliateCommissionEditModel.PaidOn = commissionPaidOn;
        }

        return affiliateCommissionEditModel;
    }

    #endregion
}