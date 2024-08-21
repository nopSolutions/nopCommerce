using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class TaskSchedulerTests : ServiceTest
{
    private TestTaskScheduler _taskScheduler;
    private IRepository<ScheduleTask> _scheduleTaskRepository;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _taskScheduler = GetService<ITaskScheduler>() as TestTaskScheduler;
        _scheduleTaskRepository = GetService<IRepository<ScheduleTask>>();

        var item = await _scheduleTaskRepository.GetByIdAsync(1);
        item.LastStartUtc = DateTime.UtcNow.AddSeconds(-(item.Seconds + 1));
        await _scheduleTaskRepository.UpdateAsync(item);

        item = await _scheduleTaskRepository.GetByIdAsync(2);
        item.LastStartUtc = DateTime.UtcNow.AddSeconds(item.Seconds - 1);
        await _scheduleTaskRepository.UpdateAsync(item);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _taskScheduler.StopSchedulerAsync();
    }

    [Test]
    public async Task CanInitialize()
    {
        _taskScheduler.IsInit.Should().BeFalse();
        await _taskScheduler.InitializeAsync();
        _taskScheduler.IsInit.Should().BeTrue();
    }

    [Test]
    public async  Task CanStartStopScheduler()
    {
        await _taskScheduler.InitializeAsync();
        _taskScheduler.IsRun.Should().BeFalse();
        await _taskScheduler.StartSchedulerAsync();
        _taskScheduler.IsRun.Should().BeTrue();
        await _taskScheduler.StopSchedulerAsync();
        _taskScheduler.IsRun.Should().BeFalse();
    }
}