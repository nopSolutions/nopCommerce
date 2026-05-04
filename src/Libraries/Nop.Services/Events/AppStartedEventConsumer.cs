using Nop.Core.Events;
using Nop.Services.Integration;
using Nop.Services.Logging;

namespace Nop.Services.Events;

/// <summary>
/// Spike: Writes an integration event to outbox when application starts
/// </summary>
public partial class AppStartedEventConsumer : IConsumer<AppStartedEvent>
{
    #region Fields

    private readonly ISpikeOutboxService _spikeOutboxService;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public AppStartedEventConsumer(
        ISpikeOutboxService spikeOutboxService,
        ILogger logger)
    {
        _spikeOutboxService = spikeOutboxService;
        _logger = logger;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the application started event
    /// </summary>
    public async Task HandleEventAsync(AppStartedEvent eventMessage)
    {
        await _logger.InformationAsync("Spike: AppStartedEvent received, writing to outbox");

        await _spikeOutboxService.WriteStartupEventAsync();
    }

    #endregion
}
