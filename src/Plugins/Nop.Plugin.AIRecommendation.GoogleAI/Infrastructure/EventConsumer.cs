using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Plugin.AIRecommendation.GoogleAI.Services;
using Nop.Services.Events;

namespace Nop.Plugin.AIRecommendation.GoogleAI.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer :
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityInsertedEvent<Product>>,
    IConsumer<EntityDeletedEvent<Product>>,
    IConsumer<EntityDeletedEvent<ProductAttributeCombination>>,
    IConsumer<EntityInsertedEvent<ProductAttributeCombination>>,
    IConsumer<EntityUpdatedEvent<ProductAttributeCombination>>
{
    #region Fields

    private readonly GoogleAiSettings _googleAiSettings;
    private readonly GoogleAiService _googleAiService;

    #endregion

    #region Ctor

    public EventConsumer(GoogleAiSettings googleAiSettings, GoogleAiService googleAiService)
    {
        _googleAiSettings = googleAiSettings;
        _googleAiService = googleAiService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.UpdateProductAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.DeleteProductAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.CreateProductAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.CreateProductAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.DeleteProductAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!_googleAiSettings.Enabled || !_googleAiSettings.SyncAllowed)
            return;

        await _googleAiService.UpdateProductAsync(eventMessage.Entity);
    }

    #endregion
}