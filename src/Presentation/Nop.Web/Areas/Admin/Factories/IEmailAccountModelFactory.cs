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
        /// <returns>Email account search model</returns>
        EmailAccountSearchModel PrepareEmailAccountSearchModel(EmailAccountSearchModel searchModel);

        /// <summary>
        /// Prepare paged email account list model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>Email account list model</returns>
        EmailAccountListModel PrepareEmailAccountListModel(EmailAccountSearchModel searchModel);

        /// <summary>
        /// Prepare email account model
        /// </summary>
        /// <param name="model">Email account model</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Email account model</returns>
        EmailAccountModel PrepareEmailAccountModel(EmailAccountModel model,
            EmailAccount emailAccount, bool excludeProperties = false);
    }
}