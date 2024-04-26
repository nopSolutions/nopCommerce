using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the return request model factory
/// </summary>
public partial interface IReturnRequestModelFactory
{
    /// <summary>
    /// Prepare return request search model
    /// </summary>
    /// <param name="searchModel">Return request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request search model
    /// </returns>
    Task<ReturnRequestSearchModel> PrepareReturnRequestSearchModelAsync(ReturnRequestSearchModel searchModel);

    /// <summary>
    /// Prepare paged return request list model
    /// </summary>
    /// <param name="searchModel">Return request search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request list model
    /// </returns>
    Task<ReturnRequestListModel> PrepareReturnRequestListModelAsync(ReturnRequestSearchModel searchModel);

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
    Task<ReturnRequestModel> PrepareReturnRequestModelAsync(ReturnRequestModel model,
        ReturnRequest returnRequest, bool excludeProperties = false);

    /// <summary>
    /// Prepare return request reason search model
    /// </summary>
    /// <param name="searchModel">Return request reason search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason search model
    /// </returns>
    Task<ReturnRequestReasonSearchModel> PrepareReturnRequestReasonSearchModelAsync(ReturnRequestReasonSearchModel searchModel);

    /// <summary>
    /// Prepare paged return request reason list model
    /// </summary>
    /// <param name="searchModel">Return request reason search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request reason list model
    /// </returns>
    Task<ReturnRequestReasonListModel> PrepareReturnRequestReasonListModelAsync(ReturnRequestReasonSearchModel searchModel);

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
    Task<ReturnRequestReasonModel> PrepareReturnRequestReasonModelAsync(ReturnRequestReasonModel model,
        ReturnRequestReason returnRequestReason, bool excludeProperties = false);

    /// <summary>
    /// Prepare return request action search model
    /// </summary>
    /// <param name="searchModel">Return request action search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action search model
    /// </returns>
    Task<ReturnRequestActionSearchModel> PrepareReturnRequestActionSearchModelAsync(ReturnRequestActionSearchModel searchModel);

    /// <summary>
    /// Prepare paged return request action list model
    /// </summary>
    /// <param name="searchModel">Return request action search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return request action list model
    /// </returns>
    Task<ReturnRequestActionListModel> PrepareReturnRequestActionListModelAsync(ReturnRequestActionSearchModel searchModel);

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
    Task<ReturnRequestActionModel> PrepareReturnRequestActionModelAsync(ReturnRequestActionModel model,
        ReturnRequestAction returnRequestAction, bool excludeProperties = false);
}