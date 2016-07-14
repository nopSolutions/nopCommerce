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
        private ICacheManager _cacheManager;
        private IRepository<ActivityLog> _activityLogRepository;
        private IRepository<ActivityLogType> _activityLogTypeRepository;
        private IWorkContext _workContext;
        private ICustomerActivityService _customerActivityService;
        private ActivityLogType _activityType1, _activityType2;
        private ActivityLog _activity1, _activity2;
        private Customer _customer1, _customer2;
        private IWebHelper _webHelper;

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
            _customer1 = new Customer
            {
                Id = 1,
                Email = "test1@teststore1.com",
                Username = "TestUser1",
                Deleted = false,
            };
           _customer2 = new Customer
           {
               Id = 2,
               Email = "test2@teststore2.com",
               Username = "TestUser2",
               Deleted = false,
           };
            _activity1 = new ActivityLog
            {
                Id = 1,
                ActivityLogType = _activityType1,
                CustomerId = _customer1.Id,
                Customer = _customer1
            };
            _activity2 = new ActivityLog
            {
                Id = 2,
                ActivityLogType = _activityType1,
                CustomerId = _customer2.Id,
                Customer = _customer2
            };
            _cacheManager = new NopNullCache();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();
            _activityLogRepository = MockRepository.GenerateMock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = MockRepository.GenerateMock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Expect(x => x.Table).Return(new List<ActivityLogType> { _activityType1, _activityType2 }.AsQueryable());
            _activityLogRepository.Expect(x => x.Table).Return(new List<ActivityLog> { _activity1, _activity2 }.AsQueryable());
            _customerActivityService = new CustomerActivityService(_cacheManager, _activityLogRepository, _activityLogTypeRepository, _workContext, null, null, null, _webHelper);
        }

        [Test]
        public void Can_Find_Activities()
        {
            var activities = _customerActivityService.GetAllActivities(null, null, 1, 0,0,10);
            activities.Contains(_activity1).ShouldBeTrue();
            activities = _customerActivityService.GetAllActivities(null, null, 2, 0, 0, 10);
            activities.Contains(_activity1).ShouldBeFalse();
            activities = _customerActivityService.GetAllActivities(null, null, 2, 0, 0, 10);
            activities.Contains(_activity2).ShouldBeTrue();
        }
    }
}
