using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the email account model factory
    /// </summary>
    public partial interface IEmailAccountModelFactory
    {
        /// <summary>
        /// Prepare email account search model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account search model
        /// </returns>
        Task<EmailAccountSearchModel> PrepareEmailAccountSearchModelAsync(EmailAccountSearchModel searchModel);

        /// <summary>
        /// Prepare paged email account list model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account list model
        /// </returns>
        Task<EmailAccountListModel> PrepareEmailAccountListModelAsync(EmailAccountSearchModel searchModel);

        /// <summary>
        /// Prepare email account model
        /// </summary>
        /// <param name="model">Email account model</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account model
        /// </returns>
        Task<EmailAccountModel> PrepareEmailAccountModelAsync(EmailAccountModel model,
            EmailAccount emailAccount, bool excludeProperties = false);
    }
}