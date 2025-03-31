using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;
public partial interface INewsLetterSubscriptionTypeService
{
    #region NewsLetterSubscriptionType

    /// <summary>
    /// Inserts a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType);

    /// <summary>
    /// Updates a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType);

    /// <summary>
    /// Deletes a newsletter subscription type
    /// </summary>
    /// <param name="newsLetterSubscriptionType">NewsLetter subscription type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType);

    /// <summary>
    /// Gets a newsletter subscription type by newsletter subscription type identifier
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeId">The newsletter subscription type identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription type
    /// </returns>
    Task<NewsLetterSubscriptionType> GetNewsLetterSubscriptionTypeByIdAsync(int newsLetterSubscriptionTypeId);

    /// <summary>
    /// Gets the newsletter subscription type list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetterSubscriptionType entities
    /// </returns>
    Task<IList<NewsLetterSubscriptionType>> GetAllNewsLetterSubscriptionTypesAsync();

    /// <summary>
    /// Gets list of subscription types by newsletter subscription
    /// </summary>
    /// <param name="newsletter">Newsletter subscription</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    IList<NewsLetterSubscriptionType> GetSubscriptionTypesByNewsLetter(NewsLetterSubscription newsletter);

    #endregion

    #region NewsLetterSubscriptionTypeMapping

    /// <summary>
    /// Get newsLetter subscription type mappings by subscription type identifier
    /// </summary>
    /// <param name="subscriptionTypeId">The newsLetter subscription type identifier</param>
    /// <param name="subscriptionId">The newsLetter subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription type mappings collection
    /// </returns>
    Task<IList<NewsLetterSubscriptionTypeMapping>> GeNewsLetterSubscriptionTypeMappingsAsync(int subscriptionTypeId, int subscriptionId = 0);

    /// <summary>
    /// Insert a newsLetter subscription type mapping
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeMapping">NewsLetter subscription type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscriptionTypeMapping newsLetterSubscriptionTypeMapping);

    /// <summary>
    /// Delete a newsLetter subscription type mapping
    /// </summary>
    /// <param name="newsLetterSubscriptionTypeMapping">NewsLetter subscription type mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscriptionTypeMapping newsLetterSubscriptionTypeMapping);

    /// <summary>
    /// Clear all newsLetter subscription type mappings
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ClearNewsLetterSubscriptionTypeMappingsAsync(NewsLetterSubscription subscription);

    #endregion
}
