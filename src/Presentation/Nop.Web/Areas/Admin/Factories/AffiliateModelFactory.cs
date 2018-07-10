using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the affiliate model factory implementation
    /// </summary>
    public partial class AffiliateModelFactory : IAffiliateModelFactory
    {
        #region Fields

        private readonly IAffiliateService _affiliateService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public AffiliateModelFactory(IAffiliateService affiliateService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPriceFormatter priceFormatter)
        {
            this._affiliateService = affiliateService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._priceFormatter = priceFormatter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
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
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare affiliated order search model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order search model</returns>
        protected virtual AffiliatedOrderSearchModel PrepareAffiliatedOrderSearchModel(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            searchModel.AffliateId = affiliate.Id;

            //prepare available order, payment and shipping statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);
            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);
            _baseAdminModelFactory.PrepareShippingStatuses(searchModel.AvailableShippingStatuses);

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
        public virtual AffiliateSearchModel PrepareAffiliateSearchModel(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged affiliate list model
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Affiliate list model</returns>
        public virtual AffiliateListModel PrepareAffiliateListModel(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get affiliates
            var affiliates = _affiliateService.GetAllAffiliates(searchModel.SearchFriendlyUrlName,
                searchModel.SearchFirstName,
                searchModel.SearchLastName,
                searchModel.LoadOnlyWithOrders,
                searchModel.OrdersCreatedFromUtc,
                searchModel.OrdersCreatedToUtc,
                searchModel.Page - 1, searchModel.PageSize, true);

            //prepare list model
            var model = new AffiliateListModel
            {
                //fill in model values from the entity
                Data = affiliates.Select(affiliate =>
                {
                    var affiliateModel = affiliate.ToModel<AffiliateModel>();
                    affiliateModel.Address = affiliate.Address.ToModel<AddressModel>();

                    return affiliateModel;
                }),
                Total = affiliates.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare affiliate model
        /// </summary>
        /// <param name="model">Affiliate model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Affiliate model</returns>
        public virtual AffiliateModel PrepareAffiliateModel(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (affiliate != null)
            {
                model = model ?? affiliate.ToModel<AffiliateModel>();
                model.Url = _affiliateService.GenerateUrl(affiliate);

                //prepare nested search models
                PrepareAffiliatedOrderSearchModel(model.AffiliatedOrderSearchModel, affiliate);
                PrepareAffiliatedCustomerSearchModel(model.AffiliatedCustomerSearchModel, affiliate);

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.AdminComment = affiliate.AdminComment;
                    model.FriendlyUrlName = affiliate.FriendlyUrlName;
                    model.Active = affiliate.Active;
                    model.Address = affiliate.Address.ToModel(model.Address);
                }
            }

            //prepare address model
            PrepareAddressModel(model.Address, affiliate?.Address);

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated order list model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order list model</returns>
        public virtual AffiliatedOrderListModel PrepareAffiliatedOrderListModel(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
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
            var orders = _orderService.SearchOrders(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                affiliateId: affiliate.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AffiliatedOrderListModel
            {
                //fill in model values from the entity
                Data = orders.Select(order => new AffiliatedOrderModel
                {
                    Id = order.Id,
                    OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                    OrderStatusId = order.OrderStatusId,
                    PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus),
                    ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                    OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    CustomOrderNumber = order.CustomOrderNumber
                }),
                Total = orders.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated customer list model
        /// </summary>
        /// <param name="searchModel">Affiliated customer search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated customer list model</returns>
        public virtual AffiliatedCustomerListModel PrepareAffiliatedCustomerListModel(AffiliatedCustomerSearchModel searchModel,
            Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //get customers
            var customers = _customerService.GetAllCustomers(affiliateId: affiliate.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AffiliatedCustomerListModel
            {
                //fill in model values from the entity
                Data = customers.Select(customer => new AffiliatedCustomerModel
                {
                    Id = customer.Id,
                    Name = customer.Email
                }),
                Total = customers.TotalCount
            };

            return model;
        }

        #endregion
    }
}