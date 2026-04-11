using AwesomeAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks;

[TestFixture]
public class ScheduleTaskServiceTests : ServiceTest
{
    private IScheduleTaskService _scheduleTaskService;
    private ScheduleTask _task;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _scheduleTaskService = GetService<IScheduleTaskService>();
        _task = new ScheduleTask
        {
            Enabled = false,
            Seconds = 1,
            Name = "test schedule task",
            Type = typeof(TestScheduleTask).FullName
        };

        await _scheduleTaskService.InsertTaskAsync(_task);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _scheduleTaskService.DeleteTaskAsync(_task);
    }

    #region CRUD tests

    [Test]
    public async Task CanInsertAndGetTask()
    {
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);

        _task.Id.Should().NotBe(0);
        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
    }

    [Test]
    public void InsertTaskShouldRaiseExceptionIfTaskIsNull()
    {
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.InsertTaskAsync(null).Wait());
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsZero()
    {
        var task = await _scheduleTaskService.GetTaskByIdAsync(0);
        task.Should().BeNull();
    }

    [Test]
    public async Task GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()
    {
        var task = await _scheduleTaskService.GetTaskByIdAsync(int.MaxValue);
        task.Should().BeNull();
    }

    [Test]
    public async Task CanUpdateTask()
    {
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        task.Name = "new test name";
        await _scheduleTaskService.UpdateTaskAsync(task);
        var task2 = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        await _scheduleTaskService.DeleteTaskAsync(_task);

        task.Id.Should().Be(task2.Id);
        task2.Name.Should().NotBe(_task.Name);
    }

    [Test]
    public void UpdateTaskShouldRaiseExceptionIfTaskIsNull()
    {
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.UpdateTaskAsync(null).Wait());
    }

    public async Task CanDeleteTask()
    {
        _task.Id = 0;
        await _scheduleTaskService.InsertTaskAsync(_task);
        await _scheduleTaskService.DeleteTaskAsync(_task);
        var task = await _scheduleTaskService.GetTaskByIdAsync(_task.Id);
        task.Should().BeNull();
    }

    [Test]
    public void DeleteTaskShouldRaiseExceptionIfTaskIsNull()
    {
        Assert.Throws<AggregateException>(() =>
            _scheduleTaskService.DeleteTaskAsync(null).Wait());
    }

    #endregion

    [Test]
    public async Task CanGetTaskByType()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(typeof(TestScheduleTask).FullName);

        task.Id.Should().Be(_task.Id);
        task.Name.Should().Be(_task.Name);
    }

    [Test]
    public async Task GetTaskByTypeAsyncShouldReturnNullIfTypeEmptyOrNotExists()
    {
        var task = await _scheduleTaskService.GetTaskByTypeAsync(null);
        task.Should().BeNull();

        task = await _scheduleTaskService.GetTaskByTypeAsync(string.Empty);
        task.Should().BeNull();

        task = await _scheduleTaskService.GetTaskByTypeAsync("not exists task type");
        task.Should().BeNull();
    }

    [Test]
    public async Task CanGetAllTasksAsync()
    {
        // GetAllTasksAsync() should return only enabled tasks
        var tasks = await _scheduleTaskService.GetAllTasksAsync();

        tasks.Should().NotBeNull();
        tasks.Count.Should().BeGreaterThan(4);
        
        // Verify only enabled tasks are returned
        tasks.Any(p => p.Enabled == false).Should().BeFalse();
        
        // Verify the disabled test task is not included
        tasks.Any(p => p.Id == _task.Id).Should().BeFalse();
        
        // Verify core enabled tasks exist
        tasks.Should().Contain(t => t.Type == "Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services");
        tasks.Should().Contain(t => t.Type == "Nop.Services.Common.KeepAliveTask, Nop.Services");
        tasks.Should().Contain(t => t.Type == "Nop.Services.Directory.UpdateExchangeRateTask, Nop.Services");
        tasks.Should().Contain(t => t.Type == "Nop.Services.Orders.CancelUnpaidOrdersTask, Nop.Services");

        // GetAllTasksAsync(true) should return all tasks including disabled ones
        tasks = await _scheduleTaskService.GetAllTasksAsync(true);

        tasks.Should().NotBeNull();
        tasks.Count.Should().BeGreaterThan(8);
        
        // Should contain both enabled and disabled tasks
        tasks.Any(p => p.Enabled).Should().BeTrue();
        
        // Should include the disabled test task
        tasks.Any(p => p.Id == _task.Id).Should().BeTrue();
        
        // Verify disabled tasks are included
        tasks.Should().Contain(t => t.Type == "Nop.Services.Caching.ClearCacheTask, Nop.Services");
        tasks.Should().Contain(t => t.Type == "Nop.Services.Logging.ClearLogTask, Nop.Services");
    }
}