using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Test.ProductProvider.Services;

public class SyncDataTask : IScheduleTask
{
    public Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}