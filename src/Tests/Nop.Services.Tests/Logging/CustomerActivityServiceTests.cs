using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Logging
{
    [TestFixture]
    public class CustomerActivityServiceTests : ServiceTest
    {
        private IStaticCacheManager _cacheManager;
        private Mock<IRepository<ActivityLog>> _activityLogRepository;
        private Mock<IRepository<ActivityLogType>> _activityLogTypeRepository;
        private Mock<IWorkContext> _workContext;
        private ICustomerActivityService _customerActivityService;
        private ActivityLogType _activityType1, _activityType2;
        private ActivityLog _activity1, _activity2;
        private Customer _customer1, _customer2;
        private Mock<IWebHelper> _webHelper;

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
                Deleted = false
            };
           _customer2 = new Customer
           {
               Id = 2,
               Email = "test2@teststore2.com",
               Username = "TestUser2",
               Deleted = false
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
                ActivityLogType = _activityType2,
                CustomerId = _customer2.Id,
                Customer = _customer2
            };
            _cacheManager = new TestCacheManager();
            _workContext = new Mock<IWorkContext>();
            _webHelper = new Mock<IWebHelper>();
            _activityLogRepository = new Mock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = new Mock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Setup(x => x.Table).Returns(new List<ActivityLogType> { _activityType1, _activityType2 }.AsQueryable());
            _activityLogRepository.Setup(x => x.Table).Returns(new List<ActivityLog> { _activity1, _activity2 }.AsQueryable());
            _customerActivityService = new CustomerActivityService(null, _activityLogRepository.Object, _activityLogTypeRepository.Object, _cacheManager, _webHelper.Object, _workContext.Object);
        }

        [Test]
        public void Can_Find_Activities()
        {
            var activities = _customerActivityService.GetAllActivities(customerId: 1, pageSize: 10);
            activities.Contains(_activity1).ShouldBeTrue();

            activities = _customerActivityService.GetAllActivities(customerId: 2, pageSize: 10);
            activities.Contains(_activity1).ShouldBeFalse();

            activities = _customerActivityService.GetAllActivities(customerId: 2, pageSize: 10);
            activities.Contains(_activity2).ShouldBeTrue();
        }
    }
}
