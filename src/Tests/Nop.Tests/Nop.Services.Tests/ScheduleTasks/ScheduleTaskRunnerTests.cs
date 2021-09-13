using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks
{
    [TestFixture]
    public class ScheduleTaskRunnerTests : ServiceTest
    {
        private IScheduleTaskService _scheduleTaskService;
        private ScheduleTask _task;
        private IScheduleTaskRunner _scheduleTaskRunner;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _scheduleTaskService = GetService<IScheduleTaskService>();
            _task = new ScheduleTask
            {
                Enabled = true,
                Seconds = 600,
                Name = "test schedule task",
                Type = typeof(TestScheduleTask).FullName
            };

            await _scheduleTaskService.InsertTaskAsync(_task);

            _scheduleTaskRunner = GetService<IScheduleTaskRunner>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _scheduleTaskService.DeleteTaskAsync(_task);
        }

        [Test]
        public async Task CanExecute()
        {
            _task.Enabled = true;
            TestScheduleTask.ResetInitFlag();
            TestScheduleTask.IsInit.Should().BeFalse();
            await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
            TestScheduleTask.IsInit.Should().BeTrue();
        }

        [Test]
        public async Task CanNotExecuteNotEnabledTask()
        {
            TestScheduleTask.ResetInitFlag();
            TestScheduleTask.IsInit.Should().BeFalse();
            _task.Enabled = false;
            await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
            TestScheduleTask.IsInit.Should().BeFalse();
        }

        [Test]
        public async Task CanExecuteNotEnabledTaskIfForceRunFlagSet()
        {
            TestScheduleTask.ResetInitFlag();
            TestScheduleTask.IsInit.Should().BeFalse();
            _task.Enabled = false;
            await _scheduleTaskRunner.ExecuteAsync(_task, true, ensureRunOncePerPeriod: false);
            TestScheduleTask.IsInit.Should().BeTrue();
        }

        [Test]
        public void ExecuteShouldRaiseExceptionIfThrowExceptionFlagSet()
        {
            _task.Enabled = true;
            Assert.Throws<AggregateException>(() =>
                _scheduleTaskRunner.ExecuteAsync(_task, throwException: true, ensureRunOncePerPeriod: false).Wait());
        }

        [Test]
        public async Task CanNotExecuteMultipleTimeIfEnsureRunOncePerPeriodFlagNotSet()
        {
            TestScheduleTask.ResetInitFlag();
            TestScheduleTask.IsInit.Should().BeFalse();
            _task.Enabled = true;
            await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: false);
            TestScheduleTask.IsInit.Should().BeTrue();

            TestScheduleTask.ResetInitFlag();
            TestScheduleTask.IsInit.Should().BeFalse();
            await _scheduleTaskRunner.ExecuteAsync(_task, ensureRunOncePerPeriod: true);
            TestScheduleTask.IsInit.Should().BeFalse();
        }
    }
}
