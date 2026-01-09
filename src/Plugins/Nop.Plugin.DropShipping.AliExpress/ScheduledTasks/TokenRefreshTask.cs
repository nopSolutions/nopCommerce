using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;


namespace Nop.Plugin.DropShipping.AliExpress.ScheduledTasks;

/// <summary>
/// Scheduled task to refresh AliExpress access token before expiry
/// </summary>
public class TokenRefreshTask : IScheduleTask
{
    private readonly IAliExpressService _aliExpressService;
    private readonly ILogger _logger;
    private readonly AliExpressSettings _settings;

    public TokenRefreshTask(
        IAliExpressService aliExpressService,
        ILogger logger,
        AliExpressSettings settings)
    {
        _aliExpressService = aliExpressService;
        _logger = logger;
        _settings = settings;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            // Check if we need to refresh the token
            if (!_settings.AccessTokenExpiresOnUtc.HasValue)
                return;

            var daysUntilExpiry = (_settings.AccessTokenExpiresOnUtc.Value - DateTime.UtcNow).TotalDays;

            if (daysUntilExpiry <= _settings.TokenRefreshDaysBeforeExpiry)
            {
                await _logger.InformationAsync($"AliExpress token expires in {daysUntilExpiry:F1} days. Refreshing...");
                
                var success = await _aliExpressService.RefreshAccessTokenAsync();
                
                if (success)
                {
                    await _logger.InformationAsync("AliExpress token refreshed successfully");
                }
                else
                {
                    await _logger.ErrorAsync("Failed to refresh AliExpress token");
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error in AliExpress token refresh task", ex);
        }
    }
}
