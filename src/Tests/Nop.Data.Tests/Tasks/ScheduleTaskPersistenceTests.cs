using System;
using Nop.Core.Domain.Tasks;
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
                StopOnError = true,
                LeasedByMachineName = "LeasedByMachineName 1",
                LeasedUntilUtc = new DateTime(2009, 01, 01), 
                LastStartUtc = new DateTime(2010, 01, 01),
                LastEndUtc = new DateTime(2010, 01, 02),
                LastSuccessUtc= new DateTime(2010, 01, 03),
            };

            var fromDb = SaveAndLoadEntity(scheduleTask);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Task 1");
            fromDb.Seconds.ShouldEqual(1);
            fromDb.Type.ShouldEqual("some type 1");
            fromDb.Enabled.ShouldEqual(true);
            fromDb.StopOnError.ShouldEqual(true);
            fromDb.LeasedByMachineName.ShouldEqual("LeasedByMachineName 1");
            fromDb.LeasedUntilUtc.ShouldEqual(new DateTime(2009, 01, 01));
            fromDb.LastStartUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.LastEndUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.LastSuccessUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }
    }
}