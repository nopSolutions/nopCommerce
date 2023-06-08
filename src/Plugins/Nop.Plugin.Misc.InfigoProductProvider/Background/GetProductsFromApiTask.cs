using System.Threading.Tasks;
using Nop.Plugin.Misc.InfigoProductProvider.Services;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.InfigoProductProvider.Background;

public class GetProductsFromApiTask : IScheduleTask
{
    private readonly IInfigoProductProviderService _infigoProductProviderService;

    public GetProductsFromApiTask(IInfigoProductProviderService infigoProductProviderService)
    {
        _infigoProductProviderService = infigoProductProviderService;
    }

    public async Task ExecuteAsync()
    {
        await _infigoProductProviderService.GetApiProducts();
    }
}