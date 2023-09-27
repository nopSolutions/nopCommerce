using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerServiceTests : ServiceTest
    {
        private ICustomerService _customerService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _customerService = GetService<ICustomerService>();
            var moderator = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.ForumModeratorsRoleName);
            moderator.Active = false;
            await _customerService.UpdateCustomerRoleAsync(moderator);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            var moderator = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.ForumModeratorsRoleName);
            moderator.Active = true;
            await _customerService.UpdateCustomerRoleAsync(moderator);
        }

        [Test]
        public async Task CanCheckIsInCustomerRole()
        {
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

            var isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName, false);
            isInCustomerRole.Should().BeTrue();
            isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName);
            isInCustomerRole.Should().BeTrue();

            isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.ForumModeratorsRoleName, false);
            isInCustomerRole.Should().BeTrue();
            isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.ForumModeratorsRoleName);
            isInCustomerRole.Should().BeFalse();

            isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.GuestsRoleName, false);
            isInCustomerRole.Should().BeFalse();
            isInCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.GuestsRoleName);
            isInCustomerRole.Should().BeFalse();
        }

        [Test]
        public async Task CanCheckWhetherCustomerIsAdmin()
        {
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            var isAdmin = await _customerService.IsAdminAsync(customer);
            isAdmin.Should().BeTrue();
        }

        [Test]
        public async Task CanCheckWhetherCustomerIsForumModerator()
        {
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            var isForumModerator = await _customerService.IsForumModeratorAsync(customer, false);
            isForumModerator.Should().BeTrue();
        }

        [Test]
        public async Task CanCheckWhetherCustomerIsGuest()
        {
            var customer = await _customerService.GetCustomerByEmailAsync("builtin@search_engine_record.com");
            var isGuest = await _customerService.IsGuestAsync(customer);
            isGuest.Should().BeTrue();
        }

        [Test]
        public async Task CanCheckWhetherCustomerIsRegistered()
        {
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);

            var isRegistered = await _customerService.IsRegisteredAsync(customer);
            isRegistered.Should().BeTrue();
        }

        [Test]
        public async Task CanRemoveAddressAssignedAsBillingAddress()
        {
            var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            var addresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

            addresses.Count.Should().Be(1);

            var address = addresses[0];

            await _customerService.InsertCustomerAddressAsync(customer, address);

            var addressesByCustomer = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
            addressesByCustomer.Count.Should().Be(1);

            var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
            billingAddress.Should().NotBeNull();

            billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
            billingAddress.Id.Should().Be(address.Id);

            await _customerService.RemoveCustomerAddressAsync(customer, address);

            addressesByCustomer = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
            var countAddresses = addressesByCustomer.Count;

            var billingAddressId = customer.BillingAddressId;

            await _customerService.InsertCustomerAddressAsync(customer, address);
            customer.BillingAddressId = address.Id;

            countAddresses.Should().Be(0);
            billingAddressId.Should().BeNull();
        }
    }
}
