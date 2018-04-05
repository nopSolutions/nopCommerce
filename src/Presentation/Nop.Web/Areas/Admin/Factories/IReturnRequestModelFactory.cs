using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial interface IReturnRequestModelFactory
    {
        /// <summary>
        /// Prepare return request search model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request search model</returns>
        ReturnRequestSearchModel PrepareReturnRequestSearchModel(ReturnRequestSearchModel searchModel);

        /// <summary>
        /// Prepare paged return request list model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request list model</returns>
        ReturnRequestListModel PrepareReturnRequestListModel(ReturnRequestSearchModel searchModel);

        /// <summary>
        /// Prepare return request model
        /// </summary>
        /// <param name="model">Return request model</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request model</returns>
        ReturnRequestModel PrepareReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties = false);

        /// <summary>
        /// Prepare return request reason search model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason search model</returns>
        ReturnRequestReasonSearchModel PrepareReturnRequestReasonSearchModel(ReturnRequestReasonSearchModel searchModel);

        /// <summary>
        /// Prepare paged return request reason list model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason list model</returns>
        ReturnRequestReasonListModel PrepareReturnRequestReasonListModel(ReturnRequestReasonSearchModel searchModel);

        /// <summary>
        /// Prepare return request reason model
        /// </summary>
        /// <param name="model">Return request reason model</param>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request reason model</returns>
        ReturnRequestReasonModel PrepareReturnRequestReasonModel(ReturnRequestReasonModel model,
            ReturnRequestReason returnRequestReason, bool excludeProperties = false);

        /// <summary>
        /// Prepare return request action search model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action search model</returns>
        ReturnRequestActionSearchModel PrepareReturnRequestActionSearchModel(ReturnRequestActionSearchModel searchModel);

        /// <summary>
        /// Prepare paged return request action list model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action list model</returns>
        ReturnRequestActionListModel PrepareReturnRequestActionListModel(ReturnRequestActionSearchModel searchModel);

        /// <summary>
        /// Prepare return request action model
        /// </summary>
        /// <param name="model">Return request action model</param>
        /// <param name="returnRequestAction">Return request action</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request action model</returns>
        ReturnRequestActionModel PrepareReturnRequestActionModel(ReturnRequestActionModel model,
            ReturnRequestAction returnRequestAction, bool excludeProperties = false);
    }
}