using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the queued email model factory
    /// </summary>
    public partial interface IQueuedEmailModelFactory
    {
        /// <summary>
        /// Prepare queued email search model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email search model
        /// </returns>
        Task<QueuedEmailSearchModel> PrepareQueuedEmailSearchModelAsync(QueuedEmailSearchModel searchModel);

        /// <summary>
        /// Prepare paged queued email list model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email list model
        /// </returns>
        Task<QueuedEmailListModel> PrepareQueuedEmailListModelAsync(QueuedEmailSearchModel searchModel);

        /// <summary>
        /// Prepare queued email model
        /// </summary>
        /// <param name="model">Queued email model</param>
        /// <param name="queuedEmail">Queued email</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email model
        /// </returns>
        Task<QueuedEmailModel> PrepareQueuedEmailModelAsync(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false);
    }
}