using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the newsletter subscription type model factory
/// </summary>
public partial interface INewsLetterSubscriptionTypeModelFactory
{
    /// <summary>
    /// Prepare newsletter subscription type search model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type search model
    /// </returns>
    Task<NewsLetterSubscriptionTypeSearchModel> PrepareNewsLetterSubscriptionTypeSearchModelAsync(NewsLetterSubscriptionTypeSearchModel searchModel);

    /// <summary>
    /// Prepare paged newsletter subscription type list model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription type search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type list model
    /// </returns>
    Task<NewsLetterSubscriptionTypeListModel> PrepareNewsLetterSubscriptionTypeListModelAsync(NewsLetterSubscriptionTypeSearchModel searchModel);

    /// <summary>
    /// Prepare newsletter subscription type model
    /// </summary>
    /// <param name="model">Newsletter subscription type model</param>
    /// <param name="subscriptionType">Subscription type</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription type model
    /// </returns>
    Task<NewsLetterSubscriptionTypeModel> PrepareNewsLetterSubscriptionTypeModelAsync(NewsLetterSubscriptionTypeModel model,
        NewsLetterSubscriptionType subscriptionType, bool excludeProperties = false);
}