using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// Message template service
/// </summary>
public partial interface IMessageTemplateService
{
    /// <summary>
    /// Delete a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteMessageTemplateAsync(MessageTemplate messageTemplate);

    /// <summary>
    /// Inserts a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertMessageTemplateAsync(MessageTemplate messageTemplate);

    /// <summary>
    /// Updates a message template
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateMessageTemplateAsync(MessageTemplate messageTemplate);

    /// <summary>
    /// Gets a message template by identifier
    /// </summary>
    /// <param name="messageTemplateId">Message template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template
    /// </returns>
    Task<MessageTemplate> GetMessageTemplateByIdAsync(int messageTemplateId);

    /// <summary>
    /// Gets message templates by the name
    /// </summary>
    /// <param name="messageTemplateName">Message template name</param>
    /// <param name="storeId">Store identifier; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of message templates
    /// </returns>
    Task<IList<MessageTemplate>> GetMessageTemplatesByNameAsync(string messageTemplateName, int? storeId = null);

    /// <summary>
    /// Gets all message templates
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="keywords">Keywords to search by name, body, or subject</param>
    /// <param name="isActive">A value indicating whether to get active records; "null" to load all records; "false" to load only inactive records; "true" to load only active records</param>
    /// <param name="emailAccountId">Email account identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template list
    /// </returns>
    Task<IList<MessageTemplate>> GetAllMessageTemplatesAsync(int storeId, string keywords = null, bool? isActive = null, int emailAccountId = 0);

    /// <summary>
    /// Create a copy of message template with all depended data
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message template copy
    /// </returns>
    Task<MessageTemplate> CopyMessageTemplateAsync(MessageTemplate messageTemplate);
}