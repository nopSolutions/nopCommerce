using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.InfigoProductProvider.Background;

public class GetProductsFromApiTask : IScheduleTask
{
    private readonly IInfigoProductProviderService _infigoProductProviderService;
    private readonly ILogger<GetProductsFromApiTask> _logger;

    public GetProductsFromApiTask(IInfigoProductProviderService infigoProductProviderService, ILogger<GetProductsFromApiTask> logger)
    {
        _infigoProductProviderService = infigoProductProviderService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Executing background task for retrieving products from Api");
        
        await _infigoProductProviderService.GetApiProducts();
    }
}