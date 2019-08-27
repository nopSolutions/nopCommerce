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
        private ICustomerService _customerService;
        private Mock<IRepository<CustomerCustomerRoleMapping>> _customerCustomerRoleMappingRepository;
        private Mock<IRepository<CustomerRole>> _customerRoleRepository;

        private CustomerRole _customerRoleAdmin = new CustomerRole
        {
            Id = 1,
            Active = true,
            Name = "Administrators",
            SystemName = NopCustomerDefaults.AdministratorsRoleName
        };

        private CustomerRole _customerRoleGuests = new CustomerRole
        {
            Id = 2,
            Active = true,
            Name = "Guests",
            SystemName = NopCustomerDefaults.GuestsRoleName
        };

        private CustomerRole _customerRoleRegistered = new CustomerRole
        {
            Id = 3,
            Active = true,
            Name = "Registered",
            SystemName = NopCustomerDefaults.RegisteredRoleName
        };

        private CustomerRole _customerRoleForumModerators = new CustomerRole
        {
            Id = 4,
            Active = true,
            Name = "ForumModerators",
            SystemName = NopCustomerDefaults.ForumModeratorsRoleName
        };

        private CustomerRole _customerRole1 = new CustomerRole
        {
            Id = 5,
            Active = true,
            Name = "Test name 1",
            SystemName = "Test system name 1"
        };

        private CustomerRole _customerRole2 = new CustomerRole
        {
            Id = 6,
            Active = false,
            Name = "Test name 2",
            SystemName = "Test system name 2"
        };

        private readonly CustomerCustomerRoleMapping _clearingMapping = new CustomerCustomerRoleMapping() { Id = 0 };

        [SetUp]
        public void Setup()
        {
            _customerRoleRepository = new Mock<IRepository<CustomerRole>>();
            _customerRoleRepository.Setup(r => r.Table).Returns(Roles);

            _customerCustomerRoleMappingRepository = new Mock<IRepository<CustomerCustomerRoleMapping>>();

            var customerCustomerRoleMapping = new List<CustomerCustomerRoleMapping>();
            _customerCustomerRoleMappingRepository.Setup(r => r.Table).Returns(customerCustomerRoleMapping.AsQueryable());

            _customerCustomerRoleMappingRepository.Setup(r => r.Insert(It.IsAny<CustomerCustomerRoleMapping>()))
                .Callback(
                    (CustomerCustomerRoleMapping ccrm) => { customerCustomerRoleMapping.Add(ccrm); });

            _customerCustomerRoleMappingRepository.Setup(r => r.Delete(It.Is<CustomerCustomerRoleMapping>(m => m.Id == 0))).Callback(
                    (CustomerCustomerRoleMapping ccrm) => { customerCustomerRoleMapping.Clear(); });

            _customerService = TestCustomerService.Get(
                customerRoleRepository: _customerRoleRepository, 
                customerCustomerRoleMappingRepository: _customerCustomerRoleMappingRepository);
        }

        private IQueryable<CustomerRole> Roles()
        {
            return new List<CustomerRole> {
                _customerRoleAdmin,
                _customerRoleGuests,
                _customerRoleRegistered,
                _customerRoleForumModerators,
                _customerRole1,
                _customerRole2

            }.AsQueryable();
        }

        [Test]
        public void Can_check_IsInCustomerRole()
        {
            var customer = new Customer() { Id = 1 };

            _customerCustomerRoleMappingRepository.Object.Insert(new CustomerCustomerRoleMapping { CustomerRoleId = _customerRole1.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(new CustomerCustomerRoleMapping { CustomerRoleId = _customerRole2.Id, CustomerId = customer.Id });


            _customerService.IsInCustomerRole(customer, "Test system name 1", false).ShouldBeTrue();
            _customerService.IsInCustomerRole(customer, "Test system name 1").ShouldBeTrue();

            _customerService.IsInCustomerRole(customer, "Test system name 2", false).ShouldBeTrue();
            _customerService.IsInCustomerRole(customer, "Test system name 2").ShouldBeFalse();

            _customerService.IsInCustomerRole(customer, "Test system name 3", false).ShouldBeFalse();
            _customerService.IsInCustomerRole(customer, "Test system name 3").ShouldBeFalse();

            _customerCustomerRoleMappingRepository.Object.Delete(_clearingMapping);
        }
        [Test]
        public void Can_check_whether_customer_is_admin()
        {
            var customer = new Customer() { Id = 1 };

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleRegistered.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleGuests.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleAdmin.Id, CustomerId = customer.Id });

            _customerService.IsAdmin(customer).ShouldBeTrue();

            _customerCustomerRoleMappingRepository.Object.Delete(_clearingMapping);
        }
        [Test]
        public void Can_check_whether_customer_is_forum_moderator()
        {
            var customer = new Customer() { Id = 1 };

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleRegistered.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleGuests.Id, CustomerId = customer.Id });

            _customerService.IsForumModerator(customer).ShouldBeFalse();

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleForumModerators.Id, CustomerId = customer.Id });

            _customerService.IsForumModerator(customer).ShouldBeTrue();

            _customerCustomerRoleMappingRepository.Object.Delete(_clearingMapping);
        }

        [Test]
        public void Can_check_whether_customer_is_guest()
        {
            var customer = new Customer() { Id = 1 };

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleRegistered.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleAdmin.Id, CustomerId = customer.Id });

            _customerService.IsGuest(customer).ShouldBeFalse();

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleGuests.Id, CustomerId = customer.Id });

            _customerService.IsGuest(customer).ShouldBeTrue();

            _customerCustomerRoleMappingRepository.Object.Delete(_clearingMapping);
        }
        [Test]
        public void Can_check_whether_customer_is_registered()
        {
            var customer = new Customer();

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleGuests.Id, CustomerId = customer.Id });
            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleAdmin.Id, CustomerId = customer.Id });


            _customerService.IsRegistered(customer).ShouldBeFalse();

            _customerCustomerRoleMappingRepository.Object.Insert(
                new CustomerCustomerRoleMapping { CustomerRoleId = _customerRoleRegistered.Id, CustomerId = customer.Id });

            _customerService.IsRegistered(customer).ShouldBeTrue();

            _customerCustomerRoleMappingRepository.Object.Delete(_clearingMapping);
        }

        [Test]
        public void Can_remove_address_assigned_as_billing_address()
        {
            var customerRepo = new Mock<IRepository<Customer>>();
            var customerAddressRepo = new Mock<IRepository<Address>>();
            var customerAddressMappingRepo = new Mock<IRepository<CustomerAddressMapping>>();
            var customerCustomerRoleMappingRepo = new Mock<IRepository<CustomerCustomerRoleMapping>>();
            var customerPasswordRepo = new Mock<IRepository<CustomerPassword>>();
            var genericAttributeRepo = new Mock<IRepository<GenericAttribute>>();
            var shoppingCartRepo = new Mock<IRepository<ShoppingCartItem>>();
            var genericAttributeService = new Mock<IGenericAttributeService>();
            var eventPublisher = new Mock<IEventPublisher>();
            var customerRoleRepo = new Mock<IRepository<CustomerRole>>();


            var customer = new Customer() { Id = 1 };

            var customers = new List<Customer>
            {
                customer
            };

            var address = new Address { Id = 1 };

            var addresses = new List<Address>
            {
                address
            };

            var addressesMapping = new List<CustomerAddressMapping>();

            customerRepo.Setup(r => r.Table).Returns(customers.AsQueryable());
            customerAddressRepo.Setup(r => r.Table).Returns(addresses.AsQueryable());
            customerAddressMappingRepo.Setup(r => r.Table).Returns(addressesMapping.AsQueryable());
            customerAddressMappingRepo.Setup(r => r.Insert(It.IsAny<CustomerAddressMapping>())).Callback(
                (CustomerAddressMapping cam) => { addressesMapping.Add(cam); });
            customerAddressMappingRepo.Setup(r => r.Delete(It.IsAny<CustomerAddressMapping>())).Callback(
                (CustomerAddressMapping cam) => { addressesMapping.Remove(cam); });

            var customerService = new CustomerService(new CustomerSettings(),
                new TestCacheManager(),
                null,
                null,
                eventPublisher.Object,
                genericAttributeService.Object,
                customerAddressRepo.Object,
                customerRepo.Object,
                customerAddressMappingRepo.Object,
                customerCustomerRoleMappingRepo.Object,
                customerPasswordRepo.Object,
                customerRoleRepo.Object,
                genericAttributeRepo.Object,
                shoppingCartRepo.Object,
                new TestCacheManager(),
                null);



            customerService.InsertCustomerAddress(customer, address);

            customerService.GetAddressesByCustomerId(customer.Id).Count().ShouldEqual(1);

            customerService.InsertCustomerAddress(customer, address);

            customerService.GetAddressesByCustomerId(customer.Id).Count().ShouldEqual(1);

            customer.BillingAddressId = address.Id;

            customerService.GetCustomerBillingAddress(customer).ShouldNotBeNull();

            customerService.GetCustomerBillingAddress(customer).Id.ShouldEqual(address.Id);

            customerService.RemoveCustomerAddress(customer, address);

            customerService.GetAddressesByCustomerId(customer.Id).Count.ShouldEqual(0);

            customer.BillingAddressId.ShouldBeNull();
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
    }
}
