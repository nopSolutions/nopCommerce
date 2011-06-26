using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Logging
{
    [TestFixture]
    public class ActivityLogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_activityLogType()
        {
            var activityLogType = new ActivityLogType
                               {
                                   SystemKeyword = "SystemKeyword 1",
                                   Name = "Name 1",
                                   Enabled = true,
                               };

            var fromDb = SaveAndLoadEntity(activityLogType);
            fromDb.ShouldNotBeNull();
            fromDb.SystemKeyword.ShouldEqual("SystemKeyword 1");
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.Enabled.ShouldEqual(true);
        }

        [Test]
        public void Can_save_and_load_activityLogType_with_activityLog()
        {
            var activityLogType = new ActivityLogType
            {
                SystemKeyword = "SystemKeyword 1",
                Name = "Name 1",
                Enabled = true,
            };
            activityLogType.ActivityLog.Add
                (
                    new ActivityLog()
                    {
                        Customer = GetTestCustomer(),
                        Comment = "Comment 1",
                        CreatedOnUtc = new DateTime(2010, 01, 03)
                    }
                );
            var fromDb = SaveAndLoadEntity(activityLogType);
            fromDb.ShouldNotBeNull();

            fromDb.ActivityLog.ShouldNotBeNull();
            (fromDb.ActivityLog.Count == 1).ShouldBeTrue();
            fromDb.ActivityLog.First().Comment.ShouldEqual("Comment 1");
            fromDb.ActivityLog.First().Customer.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.ActivityLog.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
        }


        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}