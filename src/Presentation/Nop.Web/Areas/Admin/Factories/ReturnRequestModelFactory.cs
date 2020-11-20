using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

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
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;

        #endregion

        #region Ctor

        public ReturnRequestModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IOrderService orderService,
            IProductService productService,
            IReturnRequestService returnRequestService)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _customerService = customerService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _orderService = orderService;
            _productService = productService;
            _returnRequestService = returnRequestService;
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
            var model = new ReturnRequestListModel().PrepareToGrid(searchModel, returnRequests, () =>
            {
                return returnRequests.Select(returnRequest =>
                {
                    //fill in model values from the entity
                    var returnRequestModel = returnRequest.ToModel<ReturnRequestModel>();

                    var customer = _customerService.GetCustomerById(returnRequest.CustomerId);

                    //convert dates to the user time
                    returnRequestModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    returnRequestModel.CustomerInfo = _customerService.IsRegistered(customer)
                        ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    returnRequestModel.ReturnRequestStatusStr = _localizationService.GetLocalizedEnum(returnRequest.ReturnRequestStatus);
                    var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
                    if (orderItem == null)
                        return returnRequestModel;

                    var order = _orderService.GetOrderById(orderItem.OrderId);
                    var product = _productService.GetProductById(orderItem.ProductId);

                    returnRequestModel.ProductId = orderItem.ProductId;
                    returnRequestModel.ProductName = product.Name;
                    returnRequestModel.OrderId = order.Id;
                    returnRequestModel.AttributeInfo = orderItem.AttributeDescription;
                    returnRequestModel.CustomOrderNumber = order.CustomOrderNumber;

                    return returnRequestModel;
                });
            });

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
            model ??= new ReturnRequestModel
            {
                Id = returnRequest.Id,
                CustomNumber = returnRequest.CustomNumber,
                CustomerId = returnRequest.CustomerId,
                Quantity = returnRequest.Quantity
            };

            var customer = _customerService.GetCustomerById(returnRequest.CustomerId);

            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

            model.CustomerInfo = _customerService.IsRegistered(customer)
                ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.UploadedFileGuid = _downloadService.GetDownloadById(returnRequest.UploadedFileId)?.DownloadGuid ?? Guid.Empty;
            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                var order = _orderService.GetOrderById(orderItem.OrderId);
                var product = _productService.GetProductById(orderItem.ProductId);

                model.ProductId = product.Id;
                model.ProductName = product.Name;
                model.OrderId = order.Id;
                model.AttributeInfo = orderItem.AttributeDescription;
                model.CustomOrderNumber = order.CustomOrderNumber;
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
            var reasons = _returnRequestService.GetAllReturnRequestReasons().ToPagedList(searchModel);

            //prepare list model
            var model = new ReturnRequestReasonListModel().PrepareToGrid(searchModel, reasons, () =>
            {
                return reasons.Select(reason => reason.ToModel<ReturnRequestReasonModel>());
            });

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
                model ??= returnRequestReason.ToModel<ReturnRequestReasonModel>();

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
            var actions = _returnRequestService.GetAllReturnRequestActions().ToPagedList(searchModel);

            //prepare list model
            var model = new ReturnRequestActionListModel().PrepareToGrid(searchModel, actions, () =>
            {
                return actions.Select(reason => reason.ToModel<ReturnRequestActionModel>());
            });

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
                model ??= returnRequestAction.ToModel<ReturnRequestActionModel>();

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