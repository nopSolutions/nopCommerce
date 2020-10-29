using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
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
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the affiliate model factory implementation
    /// </summary>
    public partial class AffiliateModelFactory : IAffiliateModelFactory
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public AffiliateModelFactory(IAddressService addressService,
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
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set all address fields as enabled and required
            model.FirstNameEnabled = true;
            model.FirstNameRequired = true;
            model.LastNameEnabled = true;
            model.LastNameRequired = true;
            model.EmailEnabled = true;
            model.EmailRequired = true;
            model.CompanyEnabled = true;
            model.CountryEnabled = true;
            model.CountryRequired = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CountyRequired = true;
            model.CityEnabled = true;
            model.CityRequired = true;
            model.StreetAddressEnabled = true;
            model.StreetAddressRequired = true;
            model.StreetAddress2Enabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;
            model.PhoneEnabled = true;
            model.PhoneRequired = true;
            model.FaxEnabled = true;

            //prepare available countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

            //prepare available states
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare affiliated order search model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order search model</returns>
        protected virtual async Task<AffiliatedOrderSearchModel> PrepareAffiliatedOrderSearchModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            searchModel.AffliateId = affiliate.Id;

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
        protected virtual AffiliatedCustomerSearchModel PrepareAffiliatedCustomerSearchModelAsync(AffiliatedCustomerSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            searchModel.AffliateId = affiliate.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare affiliate search model
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Affiliate search model</returns>
        public virtual Task<AffiliateSearchModel> PrepareAffiliateSearchModelAsync(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged affiliate list model
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Affiliate list model</returns>
        public virtual async Task<AffiliateListModel> PrepareAffiliateListModelAsync(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get affiliates
            var affiliates = await _affiliateService.GetAllAffiliatesAsync(searchModel.SearchFriendlyUrlName,
                searchModel.SearchFirstName,
                searchModel.SearchLastName,
                searchModel.LoadOnlyWithOrders,
                searchModel.OrdersCreatedFromUtc,
                searchModel.OrdersCreatedToUtc,
                searchModel.Page - 1, searchModel.PageSize, true);

            //prepare list model
            var model = new AffiliateListModel().PrepareToGrid(searchModel, affiliates, () =>
            {
                //fill in model values from the entity
                return affiliates.Select(affiliate =>
                {
                    var address = _addressService.GetAddressByIdAsync(affiliate.AddressId).Result;

                    var affiliateModel = affiliate.ToModel<AffiliateModel>();
                    affiliateModel.Address = address.ToModel<AddressModel>();
                    affiliateModel.Address.CountryName = _countryService.GetCountryByAddressAsync(address).Result?.Name;
                    affiliateModel.Address.StateProvinceName = _stateProvinceService.GetStateProvinceByAddressAsync(address).Result?.Name;

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
        /// <returns>Affiliate model</returns>
        public virtual async Task<AffiliateModel> PrepareAffiliateModelAsync(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (affiliate != null)
            {
                model ??= affiliate.ToModel<AffiliateModel>();
                model.Url = await _affiliateService.GenerateUrlAsync(affiliate);

                //prepare nested search models
                await PrepareAffiliatedOrderSearchModelAsync(model.AffiliatedOrderSearchModel, affiliate);
                PrepareAffiliatedCustomerSearchModelAsync(model.AffiliatedCustomerSearchModel, affiliate);

                //prepare address model
                var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
                model.Address = address.ToModel(model.Address);
                await PrepareAddressModelAsync(model.Address, address);

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.AdminComment = affiliate.AdminComment;
                    model.FriendlyUrlName = affiliate.FriendlyUrlName;
                    model.Active = affiliate.Active;
                }
            }
            else
            {
                await PrepareAddressModelAsync(model.Address);
            }

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated order list model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order list model</returns>
        public virtual async Task<AffiliatedOrderListModel> PrepareAffiliatedOrderListModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //get parameters to filter orders
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var orderStatusIds = searchModel.OrderStatusId > 0 ? new List<int> { searchModel.OrderStatusId } : null;
            var paymentStatusIds = searchModel.PaymentStatusId > 0 ? new List<int> { searchModel.PaymentStatusId } : null;
            var shippingStatusIds = searchModel.ShippingStatusId > 0 ? new List<int> { searchModel.ShippingStatusId } : null;

            //get orders
            var orders = await _orderService.SearchOrdersAsync(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                affiliateId: affiliate.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AffiliatedOrderListModel().PrepareToGrid(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.Select(order =>
                {
                    var affiliatedOrderModel = order.ToModel<AffiliatedOrderModel>();

                    //fill in additional values (not existing in the entity)
                    affiliatedOrderModel.OrderStatus = _localizationService.GetLocalizedEnumAsync(order.OrderStatus).Result;
                    affiliatedOrderModel.PaymentStatus = _localizationService.GetLocalizedEnumAsync(order.PaymentStatus).Result;
                    affiliatedOrderModel.ShippingStatus = _localizationService.GetLocalizedEnumAsync(order.ShippingStatus).Result;
                    affiliatedOrderModel.OrderTotal = _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false).Result;
                    affiliatedOrderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    return affiliatedOrderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated customer list model
        /// </summary>
        /// <param name="searchModel">Affiliated customer search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated customer list model</returns>
        public virtual async Task<AffiliatedCustomerListModel> PrepareAffiliatedCustomerListModelAsync(AffiliatedCustomerSearchModel searchModel,
            Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

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

        #endregion
    }
}