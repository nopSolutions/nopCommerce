using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks
{
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
        public void TearDown()
        {
            _taskScheduler.StopScheduler();
        }

        [Test]
        public async Task CanInitialize()
        {
            _taskScheduler.IsInit.Should().BeFalse();
            await _taskScheduler.InitializeAsync();
            _taskScheduler.IsInit.Should().BeTrue();
        }

        [Test]
        public void CanStartStopScheduler()
        {
            _taskScheduler.IsRun.Should().BeFalse();
            _taskScheduler.StartScheduler();
            _taskScheduler.IsRun.Should().BeTrue();
            _taskScheduler.StopScheduler();
            _taskScheduler.IsRun.Should().BeFalse();
        }
    }
}
