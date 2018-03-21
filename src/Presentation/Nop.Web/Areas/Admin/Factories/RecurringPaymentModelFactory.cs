using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;

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
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._orderProcessingService = orderProcessingService;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare recurring payment search model
        /// </summary>
        /// <param name="model">Recurring payment search model</param>
        /// <returns>Recurring payment search model</returns>
        public virtual RecurringPaymentSearchModel PrepareRecurringPaymentSearchModel(RecurringPaymentSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model;
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
            var model = new RecurringPaymentListModel
            {
                Data = recurringPayments.Select(recurringPayment =>
                {
                    //fill in model values from the entity
                    var recurringPaymentModel = new RecurringPaymentModel
                    {
                        Id = recurringPayment.Id,
                        CycleLength = recurringPayment.CycleLength,
                        CyclePeriodId = recurringPayment.CyclePeriodId,
                        TotalCycles = recurringPayment.TotalCycles,
                        IsActive = recurringPayment.IsActive,
                        CyclesRemaining = recurringPayment.CyclesRemaining,
                        CustomerId = recurringPayment.InitialOrder.CustomerId,
                    };

                    //convert dates to the user time
                    if (recurringPayment.NextPaymentDate.HasValue)
                    {
                        recurringPaymentModel.NextPaymentDate = _dateTimeHelper
                            .ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString();
                    }
                    recurringPaymentModel.StartDate = _dateTimeHelper
                        .ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString();

                    //fill in additional values (not existing in the entity)
                    recurringPaymentModel.CyclePeriodStr = recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext);
                    recurringPaymentModel.CustomerEmail = recurringPayment.InitialOrder.Customer.IsRegistered()
                        ? recurringPayment.InitialOrder.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");

                    return recurringPaymentModel;
                }),
                Total = recurringPayments.TotalCount
            };

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
            if (recurringPayment != null)
            {
                //fill in model values from the entity
                model = model ?? new RecurringPaymentModel
                {
                    Id = recurringPayment.Id,
                    CycleLength = recurringPayment.CycleLength,
                    CyclePeriodId = recurringPayment.CyclePeriodId,
                    TotalCycles = recurringPayment.TotalCycles,
                    IsActive = recurringPayment.IsActive,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    CustomerId = recurringPayment.InitialOrder.CustomerId,
                    LastPaymentFailed = recurringPayment.LastPaymentFailed
                };

                //convert dates to the user time
                if (recurringPayment.NextPaymentDate.HasValue)
                    model.NextPaymentDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString();
                model.StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString();
                
                model.CustomerEmail = recurringPayment.InitialOrder.Customer.IsRegistered()
                    ? recurringPayment.InitialOrder.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                model.PaymentType = _paymentService
                    .GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName)
                    .GetLocalizedEnum(_localizationService, _workContext);
                model.CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment);

                //prepare nested search model
                PrepareRecurringPaymentHistorySearchModel(model.RecurringPaymentHistorySearchModel);
            }
            
            return model;
        }

        /// <summary>
        /// Prepare recurring payment history search model
        /// </summary>
        /// <param name="model">Recurring payment history search model</param>
        /// <returns>Recurring payment history search model</returns>
        public virtual RecurringPaymentHistorySearchModel PrepareRecurringPaymentHistorySearchModel(RecurringPaymentHistorySearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

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
            var recurringPayments = recurringPayment.RecurringPaymentHistory.OrderBy(historyEntry => historyEntry.CreatedOnUtc).ToList();

            //prepare list model
            var model = new RecurringPaymentHistoryListModel
            {
                Data = recurringPayments.PaginationByRequestModel(searchModel).Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var historyModel = new RecurringPaymentHistoryModel
                    {
                        Id = historyEntry.Id,
                        OrderId = historyEntry.OrderId,
                        RecurringPaymentId = historyEntry.RecurringPaymentId,
                    };

                    //convert dates to the user time
                    historyModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = _orderService.GetOrderById(historyEntry.OrderId);
                    if (order != null)
                    {
                        historyModel.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
                        historyModel.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
                        historyModel.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
                        historyModel.CustomOrderNumber = order.CustomOrderNumber;
                    }

                    return historyModel;
                }),
                Total = recurringPayments.Count
            };

            return model;
        }

        #endregion
    }
}