using System.Collections.Generic;
using FluentAssertions;
using Nop.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerServiceTests: ServiceTest
    {
        private readonly ICustomerService _customerService;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMapping;
        private readonly IRepository<Address> _customerAddressRepo;

        private readonly CustomerRole _customerRoleAdmin = new CustomerRole
        {
            Id = 1,
            Active = true,
            Name = "Administrators",
            SystemName = NopCustomerDefaults.AdministratorsRoleName
        };

        private readonly CustomerRole _customerRoleGuests = new CustomerRole
        {
            Id = 2,
            Active = true,
            Name = "Guests",
            SystemName = NopCustomerDefaults.GuestsRoleName
        };

        private readonly CustomerRole _customerRoleRegistered = new CustomerRole
        {
            Id = 3,
            Active = true,
            Name = "Registered",
            SystemName = NopCustomerDefaults.RegisteredRoleName
        };

        private readonly CustomerRole _customerRoleForumModerators = new CustomerRole
        {
            Id = 4,
            Active = true,
            Name = "ForumModerators",
            SystemName = NopCustomerDefaults.ForumModeratorsRoleName
        };

        private readonly CustomerRole _customerRole1 = new CustomerRole
        {
            Id = 5,
            Active = true,
            Name = "Test name 1",
            SystemName = "Test system name 1"
        };

        private readonly CustomerRole _customerRole2 = new CustomerRole
        {
            Id = 6,
            Active = false,
            Name = "Test name 2",
            SystemName = "Test system name 2"
        };

        public CustomerServiceTests()
        {
            _customerAddressRepo = _fakeDataStore.RegRepository(
                new List<Address>
                {
                    new Address { Id = 1 }
                });

            _customerRepo = _fakeDataStore.RegRepository(
                new List<Customer>
                {
                    new Customer { Id = 1 }
                });
            
            var customerAddressMappingRepo = _fakeDataStore.RegRepository<CustomerAddressMapping>();

            var genericAttributeRepo = _fakeDataStore.RegRepository<GenericAttribute>();

            var customerRoleRepo = _fakeDataStore.RegRepository(new FakeRepository<CustomerRole>(Roles()));
            _customerCustomerRoleMapping = _fakeDataStore.RegRepository<CustomerCustomerRoleMapping>();

            _customerService = new FakeCustomerService(
                customerRepository: _customerRepo,
                customerAddressRepository: _customerAddressRepo,
                customerAddressMappingRepository: customerAddressMappingRepo,
                customerRoleRepository: customerRoleRepo,
                customerCustomerRoleMappingRepository: _customerCustomerRoleMapping,
                gaRepository: genericAttributeRepo);
        }

        private IList<CustomerRole> Roles()
        {
            return new List<CustomerRole>
            {
                _customerRoleAdmin,
                _customerRoleGuests,
                _customerRoleRegistered,
                _customerRoleForumModerators,
                _customerRole1,
                _customerRole2
            };
        }

        [Test]
        public void Can_check_IsInCustomerRole()
        {
            RunWithTestServiceProvider(() =>
            {
                var customer = new Customer() {Id = 1};

                var rm = new List<CustomerCustomerRoleMapping>
                {
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRole1.Id,
                        CustomerId = customer.Id
                    },
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRole2.Id,
                        CustomerId = customer.Id
                    }
                };

                _customerCustomerRoleMapping.Insert(rm);

                _customerService.IsInCustomerRole(customer, "Test system name 1", false).Should().BeTrue();
                _customerService.IsInCustomerRole(customer, "Test system name 1").Should().BeTrue();

                _customerService.IsInCustomerRole(customer, "Test system name 2", false).Should().BeTrue();
                _customerService.IsInCustomerRole(customer, "Test system name 2").Should().BeFalse();

                _customerService.IsInCustomerRole(customer, "Test system name 3", false).Should().BeFalse();
                _customerService.IsInCustomerRole(customer, "Test system name 3").Should().BeFalse();

                _customerCustomerRoleMapping.Delete(rm);
            });
        }

        [Test]
        public void Can_check_whether_customer_is_admin()
        {
            RunWithTestServiceProvider(() => {
            var customer = new Customer { Id = 1 };

            var rm = new List<CustomerCustomerRoleMapping>
            {
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleRegistered.Id, CustomerId = customer.Id },
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleGuests.Id, CustomerId = customer.Id },
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleAdmin.Id, CustomerId = customer.Id }
            };

            _customerCustomerRoleMapping.Insert(rm);

            _customerService.IsAdmin(customer).Should().BeTrue();

            _customerCustomerRoleMapping.Delete(rm);
            });
        }

        [Test]
        public void Can_check_whether_customer_is_forum_moderator()
        {
            RunWithTestServiceProvider(() =>
            {
                var customer = new Customer {Id = 1};

                var rm = new List<CustomerCustomerRoleMapping>
                {
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleRegistered.Id,
                        CustomerId = customer.Id
                    },
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleGuests.Id,
                        CustomerId = customer.Id
                    }
                };

                _customerCustomerRoleMapping.Insert(rm);

                _customerService.IsForumModerator(customer).Should().BeFalse();

                var rmForumModerators = new CustomerCustomerRoleMapping
                {
                    CustomerRoleId = _customerRoleForumModerators.Id,
                    CustomerId = customer.Id
                };

                _customerCustomerRoleMapping.Insert(rmForumModerators);

                _customerService.IsForumModerator(customer).Should().BeTrue();

                _customerCustomerRoleMapping.Delete(rm);
                _customerCustomerRoleMapping.Delete(rmForumModerators);
            });
        }

        [Test]
        public void Can_check_whether_customer_is_guest()
        {
            RunWithTestServiceProvider(() =>
            {
                var customer = new Customer {Id = 1};

                var rm = new List<CustomerCustomerRoleMapping>
                {
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleRegistered.Id,
                        CustomerId = customer.Id
                    },
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleAdmin.Id,
                        CustomerId = customer.Id
                    }
                };

                _customerCustomerRoleMapping.Insert(rm);

                _customerService.IsGuest(customer).Should().BeFalse();

                var rmRoleGuest = new CustomerCustomerRoleMapping
                {
                    CustomerRoleId = _customerRoleGuests.Id,
                    CustomerId = customer.Id
                };

                _customerCustomerRoleMapping.Insert(rmRoleGuest);

                _customerService.IsGuest(customer).Should().BeTrue();

                _customerCustomerRoleMapping.Delete(rm);
                _customerCustomerRoleMapping.Delete(rmRoleGuest);
            });
        }

        [Test]
        public void Can_check_whether_customer_is_registered()
        {
            RunWithTestServiceProvider(() =>
            {
                var customer = new Customer();

                var rm = new List<CustomerCustomerRoleMapping>
                {
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleGuests.Id,
                        CustomerId = customer.Id
                    },
                    new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = _customerRoleAdmin.Id,
                        CustomerId = customer.Id
                    }
                };

                _customerCustomerRoleMapping.Insert(rm);

                _customerService.IsRegistered(customer).Should().BeFalse();

                var rmRoleRegistered = new CustomerCustomerRoleMapping
                {
                    CustomerRoleId = _customerRoleRegistered.Id,
                    CustomerId = customer.Id
                };

                _customerCustomerRoleMapping.Insert(rmRoleRegistered);

                _customerService.IsRegistered(customer).Should().BeTrue();

                _customerCustomerRoleMapping.Delete(rm);
                _customerCustomerRoleMapping.Delete(rmRoleRegistered);
            });
        }

        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            RunWithTestServiceProvider(() =>
            {
                var customer = _customerRepo.GetById(1);
                var address = _customerAddressRepo.GetById(1);

                _customerService.InsertCustomerAddress(customer, address);

                _customerService.GetAddressesByCustomerId(customer.Id).Count.Should().Be(1);

                _customerService.InsertCustomerAddress(customer, address);

                _customerService.GetAddressesByCustomerId(customer.Id).Count.Should().Be(1);

                customer.BillingAddressId = address.Id;

                _customerService.GetCustomerBillingAddress(customer).Should().NotBeNull();

                _customerService.GetCustomerBillingAddress(customer).Id.Should().Be(address.Id);

                _customerService.RemoveCustomerAddress(customer, address);

                _customerService.GetAddressesByCustomerId(customer.Id).Count.Should().Be(0);

                customer.BillingAddressId.Should().BeNull();
            });
        }
    }
}
