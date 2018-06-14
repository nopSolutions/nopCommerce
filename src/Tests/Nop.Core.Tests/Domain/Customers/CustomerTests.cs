using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Customers
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void Can_check_IsInCustomerRole()
        {
            var customer = new Customer();

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Test name 1",
                SystemName = "Test system name 1"
            });
            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = false,
                Name = "Test name 2",
                SystemName = "Test system name 2"
            });
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

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Registered",
                SystemName = SystemCustomerRoleNames.Registered
            });
            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Guests",
                SystemName = SystemCustomerRoleNames.Guests
            });

            customer.IsAdmin().ShouldBeFalse();

            customer.CustomerRoles.Add(
                new CustomerRole
                {
                    Active = true,
                    Name = "Administrators",
                    SystemName = SystemCustomerRoleNames.Administrators
                });
            customer.IsAdmin().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_forum_moderator()
        {
            var customer = new TestCustomer();

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Registered",
                SystemName = SystemCustomerRoleNames.Registered
            });
            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Guests",
                SystemName = SystemCustomerRoleNames.Guests
            });

            customer.IsForumModerator().ShouldBeFalse();

            customer.CustomerRoles.Add(
                new CustomerRole
                {
                    Active = true,
                    Name = "ForumModerators",
                    SystemName = SystemCustomerRoleNames.ForumModerators
                });
            customer.IsForumModerator().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_guest()
        {
            var customer = new Customer();

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Registered",
                SystemName = SystemCustomerRoleNames.Registered
            });

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Administrators",
                SystemName = SystemCustomerRoleNames.Administrators
            });

            customer.IsGuest().ShouldBeFalse();

            customer.CustomerRoles.Add(
                new CustomerRole
                {
                    Active = true,
                    Name = "Guests",
                    SystemName = SystemCustomerRoleNames.Guests

                }
                );
            customer.IsGuest().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_customer_is_registered()
        {
            var customer = new Customer();
            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Administrators",
                SystemName = SystemCustomerRoleNames.Administrators
            });

            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                Name = "Guests",
                SystemName = SystemCustomerRoleNames.Guests
            });

            customer.IsRegistered().ShouldBeFalse();

            customer.CustomerRoles.Add(
                new CustomerRole
                {
                    Active = true,
                    Name = "Registered",
                    SystemName = SystemCustomerRoleNames.Registered
                });
            customer.IsRegistered().ShouldBeTrue();
        }
       
        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            var customer = new TestCustomer();
            var address = new Address { Id = 1 };

            customer.AddAddresses(address);
            customer.BillingAddress  = address;

            customer.BillingAddress.ShouldBeTheSameAs(customer.Addresses.First());

            customer.RemoveAddress(address);
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
