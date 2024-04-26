using FluentAssertions;
using Nop.Services.Customers;
using Nop.Services.Logging;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Logging;

[TestFixture]
public class CustomerActivityServiceTests : ServiceTest
{
    private ICustomerActivityService _customerActivityService;
    private ICustomerService _customerService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _customerActivityService = GetService<ICustomerActivityService>();
        _customerService = GetService<ICustomerService>();
    }

    [Test]
    public async Task CanFindActivities()
    {
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

        var activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeTrue();

        customer = await _customerService.GetCustomerByEmailAsync("builtin@search_engine_record.com");

        activities = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, pageSize: 10);
        activities.Any().Should().BeFalse();
    }
}