using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerServiceTests : ServiceTest
    {
        private ICustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            _customerService = GetService<ICustomerService>();
            var moderator = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.ForumModeratorsRoleName);
            moderator.Active = false;
            _customerService.UpdateCustomerRole(moderator);
        }

        [TearDown]
        public void TearDown()
        {
            var moderator = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.ForumModeratorsRoleName);
            moderator.Active = true;
            _customerService.UpdateCustomerRole(moderator);
        }

        [Test]
        public void CanCheckIsInCustomerRole()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);

            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.AdministratorsRoleName, false).Should().BeTrue();
            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.AdministratorsRoleName).Should().BeTrue();

            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.ForumModeratorsRoleName, false).Should().BeTrue();
            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.ForumModeratorsRoleName).Should().BeFalse();

            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.GuestsRoleName, false).Should().BeFalse();
            _customerService.IsInCustomerRole(customer, NopCustomerDefaults.GuestsRoleName).Should().BeFalse();
        }

        [Test]
        public void CanCheckWhetherCustomerIsAdmin()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            _customerService.IsAdmin(customer).Should().BeTrue();
        }

        [Test]
        public void CanCheckWhetherCustomerIsForumModerator()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            _customerService.IsForumModerator(customer, false).Should().BeTrue();
        }

        [Test]
        public void CanCheckWhetherCustomerIsGuest()
        {
            var customer = _customerService.GetCustomerByEmail("builtin@search_engine_record.com");
            _customerService.IsGuest(customer).Should().BeTrue();
        }

        [Test]
        public void CanCheckWhetherCustomerIsRegistered()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);

            _customerService.IsRegistered(customer).Should().BeTrue();
        }

        [Test]
        public void CanRemoveAddressAssignedAsBillingAddress()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            var addresses = _customerService.GetAddressesByCustomerId(customer.Id);

            addresses.Count.Should().Be(1);

            var address = addresses[0];

            _customerService.InsertCustomerAddress(customer, address);

            _customerService.GetAddressesByCustomerId(customer.Id).Count.Should().Be(1);

            _customerService.GetCustomerBillingAddress(customer).Should().NotBeNull();

            _customerService.GetCustomerBillingAddress(customer).Id.Should().Be(address.Id);

            _customerService.RemoveCustomerAddress(customer, address);

            var countAddresses = _customerService.GetAddressesByCustomerId(customer.Id).Count;

            var billingAddressId = customer.BillingAddressId;

            _customerService.InsertCustomerAddress(customer, address);
            customer.BillingAddressId = address.Id;

            countAddresses.Should().Be(0);
            billingAddressId.Should().BeNull();
        }
    }
}
