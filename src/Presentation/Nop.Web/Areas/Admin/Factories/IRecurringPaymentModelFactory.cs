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
        /// <returns>Recurring payment search model</returns>
        RecurringPaymentSearchModel PrepareRecurringPaymentSearchModel(RecurringPaymentSearchModel searchModel);

        /// <summary>
        /// Prepare paged recurring payment list model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>Recurring payment list model</returns>
        RecurringPaymentListModel PrepareRecurringPaymentListModel(RecurringPaymentSearchModel searchModel);

        /// <summary>
        /// Prepare recurring payment model
        /// </summary>
        /// <param name="model">Recurring payment model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Recurring payment model</returns>
        RecurringPaymentModel PrepareRecurringPaymentModel(RecurringPaymentModel model,
            RecurringPayment recurringPayment, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged recurring payment history list model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Recurring payment history list model</returns>
        RecurringPaymentHistoryListModel PrepareRecurringPaymentHistoryListModel(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment);
    }
}