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
        /// <param name="model">Return request search model</param>
        /// <returns>Return request search model</returns>
        ReturnRequestSearchModel PrepareReturnRequestSearchModel(ReturnRequestSearchModel model);

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
    }
}