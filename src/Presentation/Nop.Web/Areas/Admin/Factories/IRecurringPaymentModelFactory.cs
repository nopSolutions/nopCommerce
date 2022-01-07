using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the recurring payment model factory
    /// </summary>
    public partial interface IRecurringPaymentModelFactory
    {
        /// <summary>
        /// Prepare recurring payment search model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment search model
        /// </returns>
        Task<RecurringPaymentSearchModel> PrepareRecurringPaymentSearchModelAsync(RecurringPaymentSearchModel searchModel);

        /// <summary>
        /// Prepare paged recurring payment list model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment list model
        /// </returns>
        Task<RecurringPaymentListModel> PrepareRecurringPaymentListModelAsync(RecurringPaymentSearchModel searchModel);

        /// <summary>
        /// Prepare recurring payment model
        /// </summary>
        /// <param name="model">Recurring payment model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment model
        /// </returns>
        Task<RecurringPaymentModel> PrepareRecurringPaymentModelAsync(RecurringPaymentModel model,
            RecurringPayment recurringPayment, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged recurring payment history list model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment history list model
        /// </returns>
        Task<RecurringPaymentHistoryListModel> PrepareRecurringPaymentHistoryListModelAsync(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment);
    }
}