using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
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
using Rhino.Mocks;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerRegistrationServiceTests : ServiceTest
    {
        private IRepository<Customer> _customerRepo;
        private IRepository<CustomerPassword> _customerPasswordRepo;
        private IRepository<CustomerRole> _customerRoleRepo;
        private IRepository<GenericAttribute> _genericAttributeRepo;
        private IRepository<Order> _orderRepo;
        private IRepository<ForumPost> _forumPostRepo;
        private IRepository<ForumTopic> _forumTopicRepo;
        private IGenericAttributeService _genericAttributeService;
        private IEncryptionService _encryptionService;
        private ICustomerService _customerService;
        private ICustomerRegistrationService _customerRegistrationService;
        private ILocalizationService _localizationService;
        private CustomerSettings _customerSettings;
        private INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private IEventPublisher _eventPublisher;
        private IStoreService _storeService;
        private RewardPointsSettings _rewardPointsSettings;
        private SecuritySettings _securitySettings;
        private IRewardPointService _rewardPointService;
        private IWorkContext _workContext;
        private IWorkflowMessageService _workflowMessageService;

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
                Enabled = false,
            };

            _encryptionService = new EncryptionService(_securitySettings);
            _customerRepo = MockRepository.GenerateMock<IRepository<Customer>>();
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
            _customerRepo.Expect(x => x.Table).Return(new List<Customer> { customer1, customer2, customer3, customer4, customer5 }.AsQueryable());

            _customerPasswordRepo = MockRepository.GenerateMock<IRepository<CustomerPassword>>();
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
            _customerPasswordRepo.Expect(x => x.Table).Return(new[] { password1, password2, password3, password4, password5 }.AsQueryable());

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _storeService = MockRepository.GenerateMock<IStoreService>();
            _customerRoleRepo = MockRepository.GenerateMock<IRepository<CustomerRole>>();
            _genericAttributeRepo = MockRepository.GenerateMock<IRepository<GenericAttribute>>();
            _orderRepo = MockRepository.GenerateMock<IRepository<Order>>();
            _forumPostRepo = MockRepository.GenerateMock<IRepository<ForumPost>>();
            _forumTopicRepo = MockRepository.GenerateMock<IRepository<ForumTopic>>();

            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            _newsLetterSubscriptionService = MockRepository.GenerateMock<INewsLetterSubscriptionService>();
            _rewardPointService = MockRepository.GenerateMock<IRewardPointService>();

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _workflowMessageService = MockRepository.GenerateMock<IWorkflowMessageService>();

            _customerService = new CustomerService(new NopNullCache(), _customerRepo, _customerPasswordRepo, _customerRoleRepo,
                _genericAttributeRepo, _orderRepo, _forumPostRepo, _forumTopicRepo,
                null, null, null, null, null,
                _genericAttributeService, null, null, _eventPublisher, _customerSettings, null);
            _customerRegistrationService = new CustomerRegistrationService(_customerService,
                _encryptionService, _newsLetterSubscriptionService, _localizationService,
                _storeService, _rewardPointService, _workContext, _genericAttributeService,
                _workflowMessageService, _eventPublisher, _rewardPointsSettings, _customerSettings);
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
            result.ShouldEqual(CustomerLoginResults.Successful); ;
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
                SystemName = SystemCustomerRoleNames.Registered
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
