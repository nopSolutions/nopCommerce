using System.Threading.Tasks;
using FluentAssertions;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks
{
    [TestFixture]
    public class TaskSchedulerTests : ServiceTest
    {
        private TestTaskScheduler _taskScheduler;

        [OneTimeSetUp]
        public void SetUp()
        {
            _taskScheduler = GetService<ITaskScheduler>() as TestTaskScheduler;
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
        public void CanStartScheduler()
        {
            _taskScheduler.IsRun.Should().BeFalse();
            _taskScheduler.StartScheduler();
            _taskScheduler.IsRun.Should().BeTrue();
        }

        [Test]
        public void CanStopScheduler()
        {
            _taskScheduler.StartScheduler();
            _taskScheduler.IsRun.Should().BeTrue();
            _taskScheduler.StopScheduler();
            _taskScheduler.IsRun.Should().BeFalse();
        }
    }
}
