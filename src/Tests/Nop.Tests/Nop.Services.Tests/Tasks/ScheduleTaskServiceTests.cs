using System;
using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Tasks
{
    [TestFixture]
    public class ScheduleTaskServiceTests : ServiceTest
    {
        private IScheduleTaskService _scheduleTaskService;

        private ScheduleTask _task;

        [OneTimeSetUp]
        public void SetUp()
        {
            _scheduleTaskService = GetService<IScheduleTaskService>();

            _task = new ScheduleTask { Enabled = true, Name = "Test task", Seconds = 60, Type = "nop.test.task" };
        }

        [OneTimeTearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            var tasks = await _scheduleTaskService.GetAllTasksAsync(true);

            foreach (var task in tasks.Where(t=>t.Type.Equals(_task.Type, StringComparison.InvariantCultureIgnoreCase))) 
                await _scheduleTaskService.DeleteTaskAsync(task);
        }

        #region CRUD tests

        [Test]
        public async System.Threading.Tasks.Task CanInsertAndGetTask()
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
        public async System.Threading.Tasks.Task GetTaskByIdShouldReturnNullIfTaskIdIsZero()
        {
            var task = await _scheduleTaskService.GetTaskByIdAsync(0);
            task.Should().BeNull();
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdShouldReturnNullIfTaskIdIsNotExists()
        {
            var task = await _scheduleTaskService.GetTaskByIdAsync(int.MaxValue);
            task.Should().BeNull();
        }

        [Test]
        public async System.Threading.Tasks.Task CanUpdateTask()
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

        public async System.Threading.Tasks.Task CanDeleteTask()
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
        public async System.Threading.Tasks.Task CanGetTaskByType()
        {
            _task.Id = 0;
            var task = await _scheduleTaskService.GetTaskByTypeAsync(_task.Type);
            task.Should().BeNull();
            await _scheduleTaskService.InsertTaskAsync(_task);
            task = await _scheduleTaskService.GetTaskByTypeAsync(_task.Type);
            await _scheduleTaskService.DeleteTaskAsync(_task);
            task.Should().NotBeNull();
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByTypeShouldReturnNullIfTypeIsNull()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(null);
            task.Should().BeNull();
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByTypeShouldReturnNullIfTypeIsEmpty()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(string.Empty);
            task.Should().BeNull();
        }

        [Test]
        public async System.Threading.Tasks.Task CanGetAllTasks()
        {
            _task.Id = 0;
            var tasks = await _scheduleTaskService.GetAllTasksAsync(true);
            tasks.Count.Should().Be(6);
            tasks = await _scheduleTaskService.GetAllTasksAsync(false);
            tasks.Count.Should().Be(4);

            await _scheduleTaskService.InsertTaskAsync(_task);
            var tasksWithHidden = await _scheduleTaskService.GetAllTasksAsync(true);
            var tasksWitoutHidden = await _scheduleTaskService.GetAllTasksAsync(false);
            await _scheduleTaskService.DeleteTaskAsync(_task);

            tasksWithHidden.Count.Should().Be(7);
            tasksWitoutHidden.Count.Should().Be(5);

            _task.Enabled = false;

            await _scheduleTaskService.InsertTaskAsync(_task);
            tasksWithHidden = await _scheduleTaskService.GetAllTasksAsync(true);
            tasksWitoutHidden = await _scheduleTaskService.GetAllTasksAsync(false);
            await _scheduleTaskService.DeleteTaskAsync(_task);
            _task.Enabled = true;

            tasksWithHidden.Count.Should().Be(7);
            tasksWitoutHidden.Count.Should().Be(4);
        }
    }
}
