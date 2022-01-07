using System;
using System.Linq;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request search model
        /// </returns>
        public virtual async Task<ReturnRequestSearchModel> PrepareReturnRequestSearchModelAsync(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available return request statuses
            await _baseAdminModelFactory.PrepareReturnRequestStatusesAsync(searchModel.ReturnRequestStatusList, false);

            //for some reason, the standard default value (0) for the "All" item is already used for the "Pending" status, so here we use -1
            searchModel.ReturnRequestStatusId = -1;
            searchModel.ReturnRequestStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = await _localizationService.GetResourceAsync("Admin.ReturnRequests.SearchReturnRequestStatus.All")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request list model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request list model
        /// </returns>
        public virtual async Task<ReturnRequestListModel> PrepareReturnRequestListModelAsync(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;

            //get return requests
            var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(customNumber: searchModel.CustomNumber,
                rs: returnRequestStatus,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ReturnRequestListModel().PrepareToGridAsync(searchModel, returnRequests, () =>
            {
                return returnRequests.SelectAwait(async returnRequest => await PrepareReturnRequestModelAsync(null, returnRequest));
            });

            return model;
        }

        /// <summary>
        /// Prepare return request model
        /// </summary>
        /// <param name="model">Return request model</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request model
        /// </returns>
        public virtual async Task<ReturnRequestModel> PrepareReturnRequestModelAsync(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties = false)
        {
            if (returnRequest == null)
                return model;

            //fill in model values from the entity
            model ??= returnRequest.ToModel<ReturnRequestModel>();

            var customer = await _customerService.GetCustomerByIdAsync(returnRequest.CustomerId);

            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

            model.CustomerInfo = await _customerService.IsRegisteredAsync(customer)
                ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
            model.UploadedFileGuid = (await _downloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId))?.DownloadGuid ?? Guid.Empty;
            model.ReturnRequestStatusStr = await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus);
            var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reason search model
        /// </returns>
        public virtual Task<ReturnRequestReasonSearchModel> PrepareReturnRequestReasonSearchModelAsync(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged return request reason list model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reason list model
        /// </returns>
        public virtual async Task<ReturnRequestReasonListModel> PrepareReturnRequestReasonListModelAsync(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request reasons
            var reasons = (await _returnRequestService.GetAllReturnRequestReasonsAsync()).ToPagedList(searchModel);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reason model
        /// </returns>
        public virtual async Task<ReturnRequestReasonModel> PrepareReturnRequestReasonModelAsync(ReturnRequestReasonModel model,
            ReturnRequestReason returnRequestReason, bool excludeProperties = false)
        {
            Func<ReturnRequestReasonLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (returnRequestReason != null)
            {
                //fill in model values from the entity
                model ??= returnRequestReason.ToModel<ReturnRequestReasonModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(returnRequestReason, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare return request action search model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request action search model
        /// </returns>
        public virtual Task<ReturnRequestActionSearchModel> PrepareReturnRequestActionSearchModelAsync(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged return request action list model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request action list model
        /// </returns>
        public virtual async Task<ReturnRequestActionListModel> PrepareReturnRequestActionListModelAsync(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request actions
            var actions = (await _returnRequestService.GetAllReturnRequestActionsAsync()).ToPagedList(searchModel);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request action model
        /// </returns>
        public virtual async Task<ReturnRequestActionModel> PrepareReturnRequestActionModelAsync(ReturnRequestActionModel model,
            ReturnRequestAction returnRequestAction, bool excludeProperties = false)
        {
            Func<ReturnRequestActionLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (returnRequestAction != null)
            {
                //fill in model values from the entity
                model ??= returnRequestAction.ToModel<ReturnRequestActionModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(returnRequestAction, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}