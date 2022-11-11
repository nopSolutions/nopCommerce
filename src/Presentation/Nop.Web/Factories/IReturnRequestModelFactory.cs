using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the return request model factory
    /// </summary>
    public partial interface IReturnRequestModelFactory
    {
        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the submit return request model
        /// </returns>
        Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model, Order order);

        /// <summary>
        /// Prepare the customer return requests model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer return requests model
        /// </returns>
        Task<CustomerReturnRequestsModel> PrepareCustomerReturnRequestsModelAsync();
    }
}
