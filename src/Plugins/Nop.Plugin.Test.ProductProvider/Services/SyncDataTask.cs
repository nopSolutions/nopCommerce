using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;
using ILogger = WebMarkupMin.Core.Loggers.ILogger;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class SyncDataTask : IScheduleTask
{
    private readonly ILogger<SyncDataTask> _logger;

    public SyncDataTask(ILogger<SyncDataTask> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync()
    {
        _logger.LogInformation("################## Sync Data Task works ###################");
        
        return Task.CompletedTask;
    }
}