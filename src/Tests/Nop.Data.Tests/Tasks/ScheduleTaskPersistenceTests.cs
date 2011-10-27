using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Tasks
{
    [TestFixture]
    public class ScheduleTaskPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_scheduleTask()
        {
            var scheduleTask = new ScheduleTask
                               {
                                   Name = "Task 1",
                                   Seconds = 1,
                                   Type = "some type 1",
                                   Enabled = true,
                                   StopOnError = true
                               };

            var fromDb = SaveAndLoadEntity(scheduleTask);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Task 1");
            fromDb.Seconds.ShouldEqual(1);
            fromDb.Type.ShouldEqual("some type 1");
            fromDb.Enabled.ShouldEqual(true);
            fromDb.StopOnError.ShouldEqual(true);
        }
    }
}