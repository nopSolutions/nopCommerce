using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the newsletter subscription model factory
/// </summary>
public partial interface INewsLetterSubscriptionModelFactory
{
    /// <summary>
    /// Prepare newsletter subscription search model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription search model
    /// </returns>
    Task<NewsLetterSubscriptionSearchModel> PrepareNewsLetterSubscriptionSearchModelAsync(NewsLetterSubscriptionSearchModel searchModel);

    /// <summary>
    /// Prepare paged newsletter subscription list model
    /// </summary>
    /// <param name="searchModel">Newsletter subscription search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription list model
    /// </returns>
    Task<NewsLetterSubscriptionListModel> PrepareNewsLetterSubscriptionListModelAsync(NewsLetterSubscriptionSearchModel searchModel);

    /// <summary>
    /// Prepare newsletter subscription model
    /// </summary>
    /// <param name="model">Newsletter subscription model</param>
    /// <param name="subscription">Subscription</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription model
    /// </returns>
    Task<NewsLetterSubscriptionModel> PrepareNewsLetterSubscriptionModelAsync(NewsLetterSubscriptionModel model,
        NewsLetterSubscription subscription, bool excludeProperties = false);
}