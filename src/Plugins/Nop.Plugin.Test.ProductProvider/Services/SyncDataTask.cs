using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class SyncDataTask : IScheduleTask
{
    private readonly ILogger<SyncDataTask> _logger;
    private readonly IExternalProductService _externalProductService;

    public SyncDataTask(ILogger<SyncDataTask> logger, IExternalProductService externalProductService)
    {
        _logger = logger;
        _externalProductService = externalProductService;
    }

    public Task ExecuteAsync()
    {
        _logger.LogInformation("Syncing data");
        _externalProductService.SyncProducts();

        return Task.CompletedTask;
    }
}