using System;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerRegistrationServiceTests : ServiceTest
    {
        private CustomerSettings _customerSettings;
        private SecuritySettings _securitySettings;
        private RewardPointsSettings _rewardPointsSettings;
        private EncryptionService _encryptionService;
        private IRepository<Customer> _customerRepo;
        private IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private IRepository<CustomerPassword> _customerPasswordRepo;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IStoreService> _storeService;
        private IRepository<CustomerRole> _customerRoleRepo;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<INewsLetterSubscriptionService> _newsLetterSubscriptionService;
        private Mock<IRewardPointService> _rewardPointService;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IWorkContext> _workContext;
        private Mock<IWorkflowMessageService> _workflowMessageService;
        private ICustomerService _customerService;
        private CustomerRegistrationService _customerRegistrationService;

        public CustomerRegistrationServiceTests()
        {
            #region customers
            var customer1 = new Customer
            {
                Id = 1,
                Username = "a@b.com",
                Email = "a@b.com",
                Active = true
            };
            var customer2 = new Customer
            {
                Id = 2,
                Username = "test@test.com",
                Email = "test@test.com",
                Active = true
            };
            var customer3 = new Customer
            {
                Id = 3,
                Username = "user@test.com",
                Email = "user@test.com",
                Active = true
            };
            var customer4 = new Customer
            {
                Id = 4,
                Username = "registered@test.com",
                Email = "registered@test.com",
                Active = true
            };
            
            #endregion

            #region passwords
            _securitySettings = new SecuritySettings
            {
                EncryptionKey = "273ece6f97dd844d"
            };

            _encryptionService = new EncryptionService(_securitySettings);

            var saltKey = _encryptionService.CreateSaltKey(5);
            var password = _encryptionService.CreatePasswordHash("password", saltKey, "SHA512");
            var password1 = new CustomerPassword
            {
                CustomerId = customer1.Id,
                PasswordFormat = PasswordFormat.Hashed,
                PasswordSalt = saltKey,
                Password = password,
                CreatedOnUtc = DateTime.UtcNow
            };
            var password2 = new CustomerPassword
            {
                CustomerId = customer2.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };
            var password3 = new CustomerPassword
            {
                CustomerId = customer3.Id,
                PasswordFormat = PasswordFormat.Encrypted,
                Password = _encryptionService.EncryptText("password"),
                CreatedOnUtc = DateTime.UtcNow
            };
            var password4 = new CustomerPassword
            {
                CustomerId = customer4.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };
            #endregion

            _customerRoleRepo = _fakeDataStore.RegRepository(new[]
            {
                new CustomerRole
                {
                    Id = 1,
                    Active = true,
                    IsSystemRole = true,
                    SystemName = NopCustomerDefaults.RegisteredRoleName
                }
            });
            _customerRepo = _fakeDataStore.RegRepository(new[] { customer1, customer2, customer3, customer4 });
            _customerPasswordRepo = _fakeDataStore.RegRepository(new[] { password1, password2, password3, password4 });
            _customerCustomerRoleMappingRepository = _fakeDataStore.RegRepository<CustomerCustomerRoleMapping>();

            _customerService = new FakeCustomerService(
                customerCustomerRoleMappingRepository: _customerCustomerRoleMappingRepository,
                customerRepository: _customerRepo,
                customerPasswordRepository: _customerPasswordRepo,
                customerRoleRepository: _customerRoleRepo);

            //AddCustomerToRegisteredRole(customer1);
            //AddCustomerToRegisteredRole(customer2);
            //AddCustomerToRegisteredRole(customer3);
            //AddCustomerToRegisteredRole(customer4);

            _rewardPointsSettings = new RewardPointsSettings
            {
                Enabled = false
            };

            _customerSettings = new CustomerSettings
            {
                UnduplicatedPasswordsNumber = 1,
                HashedPasswordFormat = "SHA512"
            };

            _storeService = new Mock<IStoreService>();

            _genericAttributeService = new Mock<IGenericAttributeService>();
            _newsLetterSubscriptionService = new Mock<INewsLetterSubscriptionService>();
            _rewardPointService = new Mock<IRewardPointService>();

            _localizationService = new Mock<ILocalizationService>();
            _workContext = new Mock<IWorkContext>();
            _workflowMessageService = new Mock<IWorkflowMessageService>();

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _customerRegistrationService = new CustomerRegistrationService(_customerSettings,
                _customerService,
                _encryptionService,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _localizationService.Object,
                _newsLetterSubscriptionService.Object,
                _rewardPointService.Object,
                _storeService.Object,
                _workContext.Object,
                _workflowMessageService.Object,
                _rewardPointsSettings);
        }

        [SetUp]
        public override void SetUp()
        {
            var nopEngine = new Mock<NopEngine>();
            nopEngine.Setup(x => x.ServiceProvider).Returns(new TestServiceProvider());
            EngineContext.Replace(nopEngine.Object);

            _fakeDataStore.ResetStore();
            MappingCustomersToRegisteredRole();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            EngineContext.Replace(null);
        }

        //[Test]
        //public void Can_register_a_customer() 
        //{
        //    var registrationRequest = CreateCustomerRegistrationRequest();
        //    var result = _customerService.RegisterCustomer(registrationRequest);

        //    result.Success.Should().BeTrue();
        //}

        //[Test]
        //public void Can_not_have_duplicate_usernames_or_emails() 
        //{
        //    var registrationRequest = CreateUserRegistrationRequest();
        //    registrationRequest.Username = "a@b.com";
        //    registrationRequest.Email = "a@b.com";

        //    var userService = new UserService(_encryptionService, _userRepo, _userSettings);
        //    var result = userService.RegisterUser(registrationRequest);

        //    result.Success.Should().BeFalse();
        //    result.Errors.Count.Should().Be(1);
        //}

        [Test]
        public void Ensure_only_registered_customers_can_login()
        {
            var result = _customerRegistrationService.ValidateCustomer("registered@test.com", "password");
            result.Should().Be(CustomerLoginResults.Successful);

            var customer = new Customer
            {
                Username = "notregistered@test.com",
                Email = "notregistered@test.com",
                Active = true
            };

            _customerService.InsertCustomer(customer);

            _customerService.InsertCustomerPassword(new CustomerPassword
            {
                CustomerId = customer.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            });

            result = _customerRegistrationService.ValidateCustomer("notregistered@test.com", "password");
            result.Should().Be(CustomerLoginResults.NotRegistered);
        }

        [Test]
        public void Can_validate_a_hashed_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("a@b.com", "password");
            result.Should().Be(CustomerLoginResults.Successful);
        }

        [Test]
        public void Can_validate_a_clear_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("test@test.com", "password");
            result.Should().Be(CustomerLoginResults.Successful);
        }

        [Test]
        public void Can_validate_an_encrypted_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("user@test.com", "password");
            result.Should().Be(CustomerLoginResults.Successful);
        }

        private void MappingCustomersToRegisteredRole()
        {
            var regRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);
            regRole.Should().NotBeNull();
            var customers = _customerService.GetAllCustomers();

            customers.Should().NotBeNull();

            foreach (var customer in customers)
            {
                _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = regRole.Id });
            }

        }

        //private void AddCustomerToRegisteredRole(Customer customer)
        //{
        //    var regRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName);

        //    _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = regRole.Id });
        //}

        [Test]
        public void Can_change_password()
        {
            var request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Clear, "password", "password");
            var result = _customerRegistrationService.ChangePassword(request);
            result.Success.Should().BeFalse();

            request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
            result = _customerRegistrationService.ChangePassword(request);
            result.Success.Should().BeTrue();

            //request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Encrypted, "password", "newpassword");
            //result = _customerRegistrationService.ChangePassword(request);
            //result.Success.ShouldEqual(true);
        }
    }
}
