using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.Forums.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class EventConsumer : IConsumer<CustomerPermanentlyDeleted>,
    IConsumer<AdditionalTokensAddedEvent>,
    IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    #region Fields

    private readonly ForumService _forumService;
    private readonly ForumSettings _forumSettings;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public EventConsumer(
        ForumService forumService,
        ForumSettings forumSettings,
        ILocalizationService localizationService)
    {
        _forumService = forumService;
        _forumSettings = forumSettings;
        _localizationService = localizationService;
    }

    #endregion

    #region Methods

    #region GDPR

    /// <summary>
    /// Handle customer permanently deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerPermanentlyDeleted eventMessage)
    {
        if (eventMessage.CustomerId <= 0)
            return;

        //forum subscriptions
        var forumSubscriptions = await _forumService.GetAllSubscriptionsAsync(eventMessage.CustomerId);
        foreach (var forumSubscription in forumSubscriptions)
            await _forumService.DeleteSubscriptionAsync(forumSubscription);
    }

    #endregion

    #region Message templates

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(AdditionalTokensAddedEvent eventMessage)
    {
        if (eventMessage.MessageTemplate == null || string.IsNullOrEmpty(eventMessage.MessageTemplate.Name))
            return Task.CompletedTask;

        if (eventMessage.MessageTemplate.Name.Equals(ForumDefaults.NEW_FORUM_TOPIC_MESSAGE, StringComparison.InvariantCultureIgnoreCase))
            eventMessage.AddTokens("%Forums.ForumURL%", "%Forums.ForumName%", "%Forums.TopicURL%", "%Forums.TopicName%");

        if (eventMessage.MessageTemplate.Name.Equals(ForumDefaults.NEW_FORUM_POST_MESSAGE, StringComparison.InvariantCultureIgnoreCase))
            eventMessage.AddTokens("%Forums.ForumURL%", "%Forums.ForumName%", "%Forums.TopicURL%", "%Forums.TopicName%", "%Forums.PostAuthor%", "%Forums.PostBody%");

        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (!_forumSettings.ForumsEnabled)
            return;

        if (eventMessage?.Model is MenuItemModel menuItemModel)
        {
            menuItemModel.AvailableStandardRoutes.Add(new()
            {
                Text = await _localizationService.GetResourceAsync("Plugins.Misc.Forums"),
                Value = ForumDefaults.Routes.Public.BOARDS
            });
        }

        if (eventMessage?.Model is CustomerNavigationModel customerNavigationModel)
        {
            if (_forumSettings.AllowCustomersToManageSubscriptions)
            {
                customerNavigationModel.CustomerNavigationItems.Add(new()
                {
                    RouteName = ForumDefaults.Routes.Public.CUSTOMER_FORUM_SUBSCRIPTIONS,
                    Title = await _localizationService.GetResourceAsync("Plugins.Misc.Forums.Account.ForumSubscriptions"),
                    Tab = ForumDefaults.ForumCustomerNavigationTab,
                    ItemClass = "forum-subscriptions"
                });
            }
        }
    }

    #endregion
}