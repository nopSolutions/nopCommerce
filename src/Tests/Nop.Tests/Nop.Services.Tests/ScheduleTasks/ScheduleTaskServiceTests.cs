using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ScheduleTasks
{
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
            var tasks = await _scheduleTaskService.GetAllTasksAsync();

            tasks.Count.Should().Be(4);
            tasks.Any(p => p.Enabled == false).Should().BeFalse();
            tasks.Any(p=>p.Id==_task.Id).Should().BeFalse();

            tasks = await _scheduleTaskService.GetAllTasksAsync(true);

            tasks.Count.Should().Be(7);
            tasks.Any(p => p.Enabled).Should().BeTrue();
            tasks.Any(p => p.Id == _task.Id).Should().BeTrue();
        }
    }
}