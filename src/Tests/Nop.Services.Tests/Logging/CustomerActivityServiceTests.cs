using System.Linq;
using FluentAssertions;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Logging
{
    [TestFixture]
    public class CustomerActivityServiceTests : ServiceTest
    {
        private ICustomerActivityService _customerActivityService;
        private ICustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            _customerActivityService = GetService<ICustomerActivityService>();
            _customerService = GetService<ICustomerService>();
        }

        [Test]
        public void CanFindActivities()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);

            var activities = _customerActivityService.GetAllActivities(customerId: customer.Id, pageSize: 10);
            activities.Any().Should().BeTrue();

            customer = _customerService.GetCustomerByEmail("builtin@search_engine_record.com");

            activities = _customerActivityService.GetAllActivities(customerId: customer.Id, pageSize: 10);
            activities.Any().Should().BeFalse();
        }
    }
}
