using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Logging
{
    [TestFixture]
    public class CustomerActivityServiceTests : ServiceTest
    {
        ICacheManager _cacheManager;
        IRepository<ActivityLog> _activityLogRepository;
        IRepository<ActivityLogType> _activityLogTypeRepository;
        IWorkContext _workContext;
        ICustomerActivityService _customerActivityService;
        ActivityLogType _activityType1, _activityType2;
        ActivityLog _activity1, _activity2;
        Customer _customer1, _customer2;
        [SetUp]
        public new void SetUp()
        {
            _activityType1 = new ActivityLogType
            {
                Id = 1,
                SystemKeyword = "TestKeyword1",
                Enabled = true,
                Name = "Test name1"
            };
            _activityType2 = new ActivityLogType
            {
                Id = 2,
                SystemKeyword = "TestKeyword2",
                Enabled = true,
                Name = "Test name2"
            };
            _customer1 = new Customer()
            {
                Email = "test1@teststore1.com",
                Username = "TestUser1",
                Deleted = false,
            };
           _customer2 = new Customer()
           {
               Email = "test2@teststore2.com",
               Username = "TestUser2",
               Deleted = false,
           };
            _activity1 = new ActivityLog()
            {
                Id = 1,
                ActivityLogType = _activityType1,
                Customer = _customer1
            };
            _activity2 = new ActivityLog()
            {
                Id = 2,
                ActivityLogType = _activityType1,
                Customer = _customer2
            };
            _cacheManager = new NopNullCache();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _activityLogRepository = MockRepository.GenerateMock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = MockRepository.GenerateMock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Expect(x => x.Table).Return(new List<ActivityLogType>() { _activityType1, _activityType2 }.AsQueryable());
            _activityLogRepository.Expect(x => x.Table).Return(new List<ActivityLog>() { _activity1, _activity2 }.AsQueryable());
            _customerActivityService = new CustomerActivityService(_cacheManager, _activityLogRepository, _activityLogTypeRepository, _workContext);
        }

        [Test]
        public void Can_Find_Activities()
        {
            var activities = _customerActivityService.GetAllActivities(null, null, "test1@teststore1.com", "TestUser1",0,0,10);
            activities.Contains(_activity1).ShouldBeTrue();
            activities = _customerActivityService.GetAllActivities(null, null, "test2@teststore2.com", "TestUser2", 0, 0, 10);
            activities.Contains(_activity1).ShouldBeFalse();
            activities = _customerActivityService.GetAllActivities(null, null, "test2@teststore2.com", "TestUser2", 0, 0, 10);
            activities.Contains(_activity2).ShouldBeTrue();
        }

        [Test]
        public void Can_Find_Activity_By_Id()
        {
            var activity = _customerActivityService.GetActivityById(1);
            activity.ShouldBeTheSameAs(_activity1);
            activity = _customerActivityService.GetActivityById(2);
            activity.ShouldBeTheSameAs(_activity2);
        }
    }
}
