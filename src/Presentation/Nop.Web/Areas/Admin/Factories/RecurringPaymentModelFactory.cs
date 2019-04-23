using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the recurring payment model factory implementation
    /// </summary>
    public partial class RecurringPaymentModelFactory : IRecurringPaymentModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public RecurringPaymentModelFactory(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IWorkContext workContext)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare recurring payment history search model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Recurring payment history search model</returns>
        protected virtual RecurringPaymentHistorySearchModel PrepareRecurringPaymentHistorySearchModel(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            searchModel.RecurringPaymentId = recurringPayment.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare recurring payment search model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>Recurring payment search model</returns>
        public virtual RecurringPaymentSearchModel PrepareRecurringPaymentSearchModel(RecurringPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged recurring payment list model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>Recurring payment list model</returns>
        public virtual RecurringPaymentListModel PrepareRecurringPaymentListModel(RecurringPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get recurringPayments
            var recurringPayments = _orderService.SearchRecurringPayments(showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new RecurringPaymentListModel().PrepareToGrid(searchModel, recurringPayments, () =>
            {
                return recurringPayments.Select(recurringPayment =>
                {
                    //fill in model values from the entity
                    var recurringPaymentModel = recurringPayment.ToModel<RecurringPaymentModel>();

                    //convert dates to the user time
                    if (recurringPayment.NextPaymentDate.HasValue)
                    {
                        recurringPaymentModel.NextPaymentDate = _dateTimeHelper
                            .ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);
                    }

                    recurringPaymentModel.StartDate = _dateTimeHelper
                        .ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                    //fill in additional values (not existing in the entity)
                    recurringPaymentModel.CustomerId = recurringPayment.InitialOrder.CustomerId;
                    recurringPaymentModel.InitialOrderId = recurringPayment.InitialOrder.Id;
                    recurringPaymentModel.CyclePeriodStr = _localizationService.GetLocalizedEnum(recurringPayment.CyclePeriod);
                    recurringPaymentModel.CustomerEmail = recurringPayment.InitialOrder.Customer.IsRegistered()
                        ? recurringPayment.InitialOrder.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");

                    return recurringPaymentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare recurring payment model
        /// </summary>
        /// <param name="model">Recurring payment model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Recurring payment model</returns>
        public virtual RecurringPaymentModel PrepareRecurringPaymentModel(RecurringPaymentModel model,
            RecurringPayment recurringPayment, bool excludeProperties = false)
        {
            if (recurringPayment == null)
                return model;

            //fill in model values from the entity
            if (model == null)
                model = recurringPayment.ToModel<RecurringPaymentModel>();

            //convert dates to the user time
            if (recurringPayment.NextPaymentDate.HasValue)
                model.NextPaymentDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);
            model.StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

            model.CustomerId = recurringPayment.InitialOrder.CustomerId;
            model.InitialOrderId = recurringPayment.InitialOrder.Id;
            model.CustomerEmail = recurringPayment.InitialOrder.Customer.IsRegistered()
                ? recurringPayment.InitialOrder.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.PaymentType = _localizationService.GetLocalizedEnum(_paymentService
                .GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName));
            model.CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment);

            //prepare nested search model
            PrepareRecurringPaymentHistorySearchModel(model.RecurringPaymentHistorySearchModel, recurringPayment);

            return model;
        }

        /// <summary>
        /// Prepare paged recurring payment history list model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Recurring payment history list model</returns>
        public virtual RecurringPaymentHistoryListModel PrepareRecurringPaymentHistoryListModel(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            //get recurring payments history
            var recurringPayments = recurringPayment.RecurringPaymentHistory
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new RecurringPaymentHistoryListModel().PrepareToGrid(searchModel, recurringPayments, () =>
            {
                return recurringPayments.Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var historyModel = historyEntry.ToModel<RecurringPaymentHistoryModel>();

                    //convert dates to the user time
                    historyModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = _orderService.GetOrderById(historyEntry.OrderId);
                    if (order == null)
                        return historyModel;

                    historyModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    historyModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    historyModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
                    historyModel.CustomOrderNumber = order.CustomOrderNumber;

                    return historyModel;
                });
            });

            return model;
        }

        #endregion
    }
}