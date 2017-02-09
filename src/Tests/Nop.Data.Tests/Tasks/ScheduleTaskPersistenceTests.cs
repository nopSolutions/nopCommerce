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
            var scheduleTask = this.GetTestScheduleTask();

            var fromDb = SaveAndLoadEntity(this.GetTestScheduleTask());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(scheduleTask);
        }
    }
}