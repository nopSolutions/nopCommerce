using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Events;

namespace Nop.Plugin.AIRecommendation.GoogleAI.Services;

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
    private readonly IAiRecommendationPluginManager _aiRecommendationPluginManager;

    private bool? _isPluginActive;

    #endregion

    #region Ctor

    public EventConsumer(GoogleAiSettings googleAiSettings,
        GoogleAiService googleAiService,
        IAiRecommendationPluginManager aiRecommendationPluginManager)
    {
        _googleAiSettings = googleAiSettings;
        _googleAiService = googleAiService;
        _aiRecommendationPluginManager = aiRecommendationPluginManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Checks whether the plugin is active and allowed to sync data
    /// </summary>
    /// <returns>True if the plugin is active and allowed to sync data; otherwise, false</returns>
    private async Task<bool> IsPluginActiveAsync()
    {
        if (!_googleAiSettings.Enabled)
            return false;

        if (!_googleAiSettings.SyncAllowed)
            return false;

        if (_isPluginActive.HasValue)
            return _isPluginActive.Value;

        //try to use AI-powered recommendation provider
        var activeAiRecommendationProvider = await _aiRecommendationPluginManager.LoadPrimaryPluginAsync();

        _isPluginActive = activeAiRecommendationProvider is GoogleAiPlugin;

        return _isPluginActive.Value;
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
        if (!await IsPluginActiveAsync())
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
        if (!await IsPluginActiveAsync())
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
        if (!await IsPluginActiveAsync())
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
        if (!await IsPluginActiveAsync())
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
        if (!await IsPluginActiveAsync())
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
        if (!await IsPluginActiveAsync())
            return;

        await _googleAiService.UpdateProductAsync(eventMessage.Entity);
    }

    #endregion
}