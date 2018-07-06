using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the return request model factory implementation
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IOrderService _orderService;
        private readonly IReturnRequestService _returnRequestService;

        #endregion

        #region Ctor

        public ReturnRequestModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IOrderService orderService,
            IReturnRequestService returnRequestService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._downloadService = downloadService;
            this._localizationService = localizationService;
            this._localizedModelFactory = localizedModelFactory;
            this._orderService = orderService;
            this._returnRequestService = returnRequestService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare return request search model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request search model</returns>
        public virtual ReturnRequestSearchModel PrepareReturnRequestSearchModel(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available return request statuses
            _baseAdminModelFactory.PrepareReturnRequestStatuses(searchModel.ReturnRequestStatusList, false);

            //for some reason, the standard default value (0) for the "All" item is already used for the "Pending" status, so here we use -1
            //TODO: move away from using 0 in ReturnRequestStatus enum
            searchModel.ReturnRequestStatusId = -1;
            searchModel.ReturnRequestStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = _localizationService.GetResource("Admin.ReturnRequests.SearchReturnRequestStatus.All")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request list model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request list model</returns>
        public virtual ReturnRequestListModel PrepareReturnRequestListModel(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;

            //get return requests
            var returnRequests = _returnRequestService.SearchReturnRequests(customNumber: searchModel.CustomNumber,
                rs: returnRequestStatus,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ReturnRequestListModel
            {
                Data = returnRequests.Select(returnRequest =>
                {
                    //fill in model values from the entity
                    var returnRequestModel = new ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        CustomNumber = returnRequest.CustomNumber,
                        CustomerId = returnRequest.CustomerId,
                        Quantity = returnRequest.Quantity
                    };

                    //convert dates to the user time
                    returnRequestModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    returnRequestModel.CustomerInfo = returnRequest.Customer.IsRegistered()
                        ? returnRequest.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    returnRequestModel.ReturnRequestStatusStr = _localizationService.GetLocalizedEnum(returnRequest.ReturnRequestStatus);
                    var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
                    if (orderItem == null)
                        return returnRequestModel;

                    returnRequestModel.ProductId = orderItem.ProductId;
                    returnRequestModel.ProductName = orderItem.Product.Name;
                    returnRequestModel.OrderId = orderItem.OrderId;
                    returnRequestModel.AttributeInfo = orderItem.AttributeDescription;
                    returnRequestModel.CustomOrderNumber = orderItem.Order.CustomOrderNumber;

                    return returnRequestModel;
                }),
                Total = returnRequests.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare return request model
        /// </summary>
        /// <param name="model">Return request model</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request model</returns>
        public virtual ReturnRequestModel PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties = false)
        {
            if (returnRequest == null)
                return model;

            //fill in model values from the entity
            model = model ?? new ReturnRequestModel
            {
                Id = returnRequest.Id,
                CustomNumber = returnRequest.CustomNumber,
                CustomerId = returnRequest.CustomerId,
                Quantity = returnRequest.Quantity
            };

            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

            model.CustomerInfo = returnRequest.Customer.IsRegistered()
                ? returnRequest.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.UploadedFileGuid = _downloadService.GetDownloadById(returnRequest.UploadedFileId)?.DownloadGuid ?? Guid.Empty;
            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                model.ProductId = orderItem.ProductId;
                model.ProductName = orderItem.Product.Name;
                model.OrderId = orderItem.OrderId;
                model.AttributeInfo = orderItem.AttributeDescription;
                model.CustomOrderNumber = orderItem.Order.CustomOrderNumber;
            }

            if (excludeProperties)
                return model;

            model.ReasonForReturn = returnRequest.ReasonForReturn;
            model.RequestedAction = returnRequest.RequestedAction;
            model.CustomerComments = returnRequest.CustomerComments;
            model.StaffNotes = returnRequest.StaffNotes;
            model.ReturnRequestStatusId = returnRequest.ReturnRequestStatusId;

            return model;
        }

        /// <summary>
        /// Prepare return request reason search model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason search model</returns>
        public virtual ReturnRequestReasonSearchModel PrepareReturnRequestReasonSearchModel(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request reason list model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason list model</returns>
        public virtual ReturnRequestReasonListModel PrepareReturnRequestReasonListModel(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request reasons
            var reasons = _returnRequestService.GetAllReturnRequestReasons();

            //prepare list model
            var model = new ReturnRequestReasonListModel
            {
                //fill in model values from the entity
                Data = reasons.PaginationByRequestModel(searchModel).Select(reason => reason.ToModel<ReturnRequestReasonModel>()),
                Total = reasons.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare return request reason model
        /// </summary>
        /// <param name="model">Return request reason model</param>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request reason model</returns>
        public virtual ReturnRequestReasonModel PrepareReturnRequestReasonModel(ReturnRequestReasonModel model,
            ReturnRequestReason returnRequestReason, bool excludeProperties = false)
        {
            Action<ReturnRequestReasonLocalizedModel, int> localizedModelConfiguration = null;

            if (returnRequestReason != null)
            {
                //fill in model values from the entity
                model = model ?? returnRequestReason.ToModel<ReturnRequestReasonModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(returnRequestReason, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare return request action search model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action search model</returns>
        public virtual ReturnRequestActionSearchModel PrepareReturnRequestActionSearchModel(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request action list model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action list model</returns>
        public virtual ReturnRequestActionListModel PrepareReturnRequestActionListModel(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request actions
            var actions = _returnRequestService.GetAllReturnRequestActions();

            //prepare list model
            var model = new ReturnRequestActionListModel
            {
                //fill in model values from the entity
                Data = actions.PaginationByRequestModel(searchModel).Select(action => action.ToModel<ReturnRequestActionModel>()),
                Total = actions.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare return request action model
        /// </summary>
        /// <param name="model">Return request action model</param>
        /// <param name="returnRequestAction">Return request action</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request action model</returns>
        public virtual ReturnRequestActionModel PrepareReturnRequestActionModel(ReturnRequestActionModel model,
            ReturnRequestAction returnRequestAction, bool excludeProperties = false)
        {
            Action<ReturnRequestActionLocalizedModel, int> localizedModelConfiguration = null;

            if (returnRequestAction != null)
            {
                //fill in model values from the entity
                model = model ?? returnRequestAction.ToModel<ReturnRequestActionModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(returnRequestAction, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}