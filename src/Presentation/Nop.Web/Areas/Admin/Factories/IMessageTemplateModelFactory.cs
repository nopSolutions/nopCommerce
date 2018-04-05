using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the message template model factory
    /// </summary>
    public partial interface IMessageTemplateModelFactory
    {
        /// <summary>
        /// Prepare message template search model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>Message template search model</returns>
        MessageTemplateSearchModel PrepareMessageTemplateSearchModel(MessageTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged message template list model
        /// </summary>
        /// <param name="searchModel">Message template search model</param>
        /// <returns>Message template list model</returns>
        MessageTemplateListModel PrepareMessageTemplateListModel(MessageTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare message template model
        /// </summary>
        /// <param name="model">Message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Message template model</returns>
        MessageTemplateModel PrepareMessageTemplateModel(MessageTemplateModel model,
            MessageTemplate messageTemplate, bool excludeProperties = false);

        /// <summary>
        /// Prepare test message template model
        /// </summary>
        /// <param name="model">Test message template model</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Test message template model</returns>
        TestMessageTemplateModel PrepareTestMessageTemplateModel(TestMessageTemplateModel model,
            MessageTemplate messageTemplate, int languageId);
    }
}