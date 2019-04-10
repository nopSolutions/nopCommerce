using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
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
        private Mock<IRepository<Customer>> _customerRepo;
        private Mock<IRepository<CustomerPassword>> _customerPasswordRepo;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IStoreService> _storeService;
        private Mock<IRepository<CustomerRole>> _customerRoleRepo;
        private Mock<IRepository<GenericAttribute>> _genericAttributeRepo;
        private Mock<IRepository<ShoppingCartItem>> _shoppingCartRepo;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<INewsLetterSubscriptionService> _newsLetterSubscriptionService;
        private Mock<IRewardPointService> _rewardPointService;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IWorkContext> _workContext;
        private Mock<IWorkflowMessageService> _workflowMessageService;
        private CustomerService _customerService;
        private CustomerRegistrationService _customerRegistrationService;
        private Mock<IRepository<CustomerCustomerRoleMapping>> _customerCustomerRoleMappingRepo;

        [SetUp]
        public new void SetUp()
        {
            _customerSettings = new CustomerSettings
            {
                UnduplicatedPasswordsNumber = 1,
                HashedPasswordFormat = "SHA512"
            };
            _securitySettings = new SecuritySettings
            {
                EncryptionKey = "273ece6f97dd844d"
            };
            _rewardPointsSettings = new RewardPointsSettings
            {
                Enabled = false
            };

            _encryptionService = new EncryptionService(_securitySettings);
            _customerRepo = new Mock<IRepository<Customer>>();

            var customer1 = new Customer
            {
                Id = 1,
                Username = "a@b.com",
                Email = "a@b.com",
                Active = true
            };
            AddCustomerToRegisteredRole(customer1);

            var customer2 = new Customer
            {
                Id = 2,
                Username = "test@test.com",
                Email = "test@test.com",
                Active = true
            };
            AddCustomerToRegisteredRole(customer2);

            var customer3 = new Customer
            {
                Id = 3,
                Username = "user@test.com",
                Email = "user@test.com",
                Active = true
            };
            AddCustomerToRegisteredRole(customer3);

            var customer4 = new Customer
            {
                Id = 4,
                Username = "registered@test.com",
                Email = "registered@test.com",
                Active = true
            };
            AddCustomerToRegisteredRole(customer4);

            var customer5 = new Customer
            {
                Id = 5,
                Username = "notregistered@test.com",
                Email = "notregistered@test.com",
                Active = true
            };
            _customerRepo.Setup(x => x.Table).Returns(new List<Customer> { customer1, customer2, customer3, customer4, customer5 }.AsQueryable());

            _customerPasswordRepo = new Mock<IRepository<CustomerPassword>>();
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
            var password5 = new CustomerPassword
            {
                CustomerId = customer5.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };
            _customerPasswordRepo.Setup(x => x.Table).Returns(new[] { password1, password2, password3, password4, password5 }.AsQueryable());

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _storeService = new Mock<IStoreService>();
            _customerRoleRepo = new Mock<IRepository<CustomerRole>>();
            _genericAttributeRepo = new Mock<IRepository<GenericAttribute>>();
            _shoppingCartRepo = new Mock<IRepository<ShoppingCartItem>>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _newsLetterSubscriptionService = new Mock<INewsLetterSubscriptionService>();
            _rewardPointService = new Mock<IRewardPointService>();

            _localizationService = new Mock<ILocalizationService>();
            _workContext = new Mock<IWorkContext>();
            _workflowMessageService = new Mock<IWorkflowMessageService>();
            _customerCustomerRoleMappingRepo = new Mock<IRepository<CustomerCustomerRoleMapping>>();
            
             _customerService = new CustomerService(new CustomerSettings(),
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

        //[Test]
        //public void Can_register_a_customer() 
        //{
        //    var registrationRequest = CreateCustomerRegistrationRequest();
        //    var result = _customerService.RegisterCustomer(registrationRequest);

        //    result.Success.ShouldBeTrue();
        //}

        //[Test]
        //public void Can_not_have_duplicate_usernames_or_emails() 
        //{
        //    var registrationRequest = CreateUserRegistrationRequest();
        //    registrationRequest.Username = "a@b.com";
        //    registrationRequest.Email = "a@b.com";

        //    var userService = new UserService(_encryptionService, _userRepo, _userSettings);
        //    var result = userService.RegisterUser(registrationRequest);

        //    result.Success.ShouldBeFalse();
        //    result.Errors.Count.ShouldEqual(1);
        //}

        [Test]
        public void Ensure_only_registered_customers_can_login()
        {
            var result = _customerRegistrationService.ValidateCustomer("registered@test.com", "password");
            result.ShouldEqual(CustomerLoginResults.Successful);

            result = _customerRegistrationService.ValidateCustomer("notregistered@test.com", "password");
            result.ShouldEqual(CustomerLoginResults.NotRegistered);
        }

        [Test]
        public void Can_validate_a_hashed_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("a@b.com", "password");
            result.ShouldEqual(CustomerLoginResults.Successful);
        }

        [Test]
        public void Can_validate_a_clear_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("test@test.com", "password");
            result.ShouldEqual(CustomerLoginResults.Successful);
        }

        [Test]
        public void Can_validate_an_encrypted_password()
        {
            var result = _customerRegistrationService.ValidateCustomer("user@test.com", "password");
            result.ShouldEqual(CustomerLoginResults.Successful);
        }

        private void AddCustomerToRegisteredRole(Customer customer)
        {
            customer.CustomerRoles.Add(new CustomerRole
            {
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.RegisteredRoleName
            });
        }

        [Test]
        public void Can_change_password()
        {
            var request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Clear, "password", "password");
            var result = _customerRegistrationService.ChangePassword(request);
            result.Success.ShouldEqual(false);

            request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
            result = _customerRegistrationService.ChangePassword(request);
            result.Success.ShouldEqual(true);

            //request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Encrypted, "password", "newpassword");
            //result = _customerRegistrationService.ChangePassword(request);
            //result.Success.ShouldEqual(true);
        }
    }
}
