using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Customers
{
    [TestFixture]
    public class CustomerTests
    {
        CustomerRole customerRoleAdmin = new CustomerRole
        {
            Active = true,
            Name = "Administrators",
            SystemName = NopCustomerDefaults.AdministratorsRoleName
        };

        CustomerRole customerRoleGuests = new CustomerRole
        {
            Active = true,
            Name = "Guests",
            SystemName = NopCustomerDefaults.GuestsRoleName
        };

        CustomerRole customerRoleRegistered = new CustomerRole
        {
            Active = true,
            Name = "Registered",
            SystemName = NopCustomerDefaults.RegisteredRoleName
        };

        [Test]
        public void Can_check_IsInCustomerRole()
        {
            var customer = new Customer();

            var customerRole1 = new CustomerRole
            {
                Active = true,
                Name = "Test name 1",
                SystemName = "Test system name 1"
            };

            var customerRole2 = new CustomerRole
            {
                Active = false,
                Name = "Test name 2",
                SystemName = "Test system name 2"
            };

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRole1 }
            );

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRole2 }
            );

            customer.IsInCustomerRole("Test system name 1", false).ShouldBeTrue();
            customer.IsInCustomerRole("Test system name 1").ShouldBeTrue();

            customer.IsInCustomerRole("Test system name 2", false).ShouldBeTrue();
            customer.IsInCustomerRole("Test system name 2").ShouldBeFalse();

            customer.IsInCustomerRole("Test system name 3", false).ShouldBeFalse();
            customer.IsInCustomerRole("Test system name 3").ShouldBeFalse();
        }
        [Test]
        public void Can_check_whether_customer_is_admin()
        {
            var customer = new Customer();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleRegistered }
            );

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleGuests }
            );

            customer.IsAdmin().ShouldBeFalse();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleAdmin }
            );

            customer.IsAdmin().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_forum_moderator()
        {
            var customer = new TestCustomer();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleRegistered }
            );

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleGuests }
            );

            customer.IsForumModerator().ShouldBeFalse();

            var customerRoleForumModerators = new CustomerRole
            {
                Active = true,
                Name = "ForumModerators",
                SystemName = NopCustomerDefaults.ForumModeratorsRoleName
            };

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleForumModerators }
            );

            customer.IsForumModerator().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_guest()
        {
            var customer = new Customer();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleRegistered }
            );

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleAdmin }
            );

            customer.IsGuest().ShouldBeFalse();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleGuests }
            );

            customer.IsGuest().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_registered()
        {
            var customer = new Customer();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleAdmin }
            );

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleGuests }
            );

            customer.IsRegistered().ShouldBeFalse();

            customer.AddCustomerRoleMapping(
                new CustomerCustomerRoleMapping { CustomerRole = customerRoleRegistered }
            );

            customer.IsRegistered().ShouldBeTrue();
        }

        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            var _customerRepo = new Mock<IRepository<Customer>>();
            var _customerCustomerRoleMappingRepo = new Mock<IRepository<CustomerCustomerRoleMapping>>();
            var _customerPasswordRepo = new Mock<IRepository<CustomerPassword>>();
            var _genericAttributeRepo = new Mock<IRepository<GenericAttribute>>();
            var _shoppingCartRepo = new Mock<IRepository<ShoppingCartItem>>();
            var _genericAttributeService = new Mock<IGenericAttributeService>();
            var _eventPublisher = new Mock<IEventPublisher>();
            var _customerRoleRepo = new Mock<IRepository<CustomerRole>>();

            var _customerService = new CustomerService(new CustomerSettings(), 
                new TestCacheManager(), 
                null,
                null,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _customerRepo.Object,
                _customerCustomerRoleMappingRepo.Object,
                _customerPasswordRepo.Object,
                _customerRoleRepo.Object,
                _genericAttributeRepo.Object,
                _shoppingCartRepo.Object,
                new TestCacheManager(),
                null);

            var customer = new TestCustomer();
            var address = new Address { Id = 1 };

            customer.AddAddresses(address);
            customer.BillingAddress  = address;

            customer.BillingAddress.ShouldBeTheSameAs(customer.Addresses.First());

            _customerService.RemoveCustomerAddress(customer, address);
            customer.Addresses.Count.ShouldEqual(0);
            customer.BillingAddress.ShouldBeNull();
        }

        [Test]
        public void Can_add_rewardPointsHistoryEntry()
        {
            //TODO temporary disabled until we can inject (not resolve using DI) "RewardPointsSettings" into "LimitPerStore" method of CustomerExtensions

            //var customer = new Customer();
            //customer.AddRewardPointsHistoryEntry(1, 0, "Points for registration");

            //customer.RewardPointsHistory.Count.ShouldEqual(1);
            //customer.RewardPointsHistory.First().Points.ShouldEqual(1);
        }

        [Test]
        public void Can_get_rewardPointsHistoryBalance()
        {
            //TODO temporary disabled until we can inject (not resolve using DI) "RewardPointsSettings" into "LimitPerStore" method of CustomerExtensions

            //var customer = new Customer();
            //customer.AddRewardPointsHistoryEntry(1, 0, "Points for registration");

            //customer.GetRewardPointsBalance(0).ShouldEqual(1);
        }

        class TestCustomer : Customer
        {
            public TestCustomer()
            {
                _customerAddressMappings = new List<CustomerAddressMapping>();
            }

            public void AddAddresses(Address address)
            {
                _customerAddressMappings.Add(new CustomerAddressMapping{Address = address, AddressId = address.Id});
            }
        }
    }
}
