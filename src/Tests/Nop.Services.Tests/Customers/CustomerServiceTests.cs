using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Messages;
using NUnit.Framework;
using Rhino.Mocks;
using Nop.Tests;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Security;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerServiceTests : ServiceTest
    {
        IRepository<Customer> _customerRepo;
        IRepository<CustomerRole> _customerRoleRepo;
        IRepository<CustomerAttribute> _customerAttributeRepo;
        IEncryptionService _encryptionService;
        ICustomerService _customerService;
        CustomerSettings _customerSettings;
        INewsLetterSubscriptionService newsLetterSubscriptionService;
        RewardPointsSettings _rewardPointsSettings;
        SecuritySettings _securitySettings;

        [SetUp]
        public void SetUp() 
        {
            _customerSettings = new CustomerSettings();
            _securitySettings = new SecuritySettings()
            {
                EncryptionKey = "273ece6f97dd844d"
            };
            _rewardPointsSettings = new RewardPointsSettings()
            {
                 Enabled = false,
            };

            _encryptionService = new EncryptionService(_securitySettings);
            _customerRepo = MockRepository.GenerateMock<IRepository<Customer>>();
            var customer = new Customer() 
            {
                Username = "a@b.com",
                Email = "a@b.com",
                PasswordFormat = PasswordFormat.Hashed,
                Active = true
            };

            string saltKey = _encryptionService.CreateSaltKey(5);
            string password = _encryptionService.CreatePasswordHash("password", saltKey);
            customer.PasswordSalt = saltKey;
            customer.Password = password;

            var customer2 = new Customer() 
            {
                Username = "test@test.com",
                Email = "test@test.com",
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                Active = true
            };

            var customer3 = new Customer() 
            {
                Username = "user@test.com",
                Email = "user@test.com",
                PasswordFormat = PasswordFormat.Encrypted,
                Password = _encryptionService.EncryptText("password"),
                Active = true
            };

            _customerRepo.Expect(x => x.Table).Return(new List<Customer>() { customer, customer2, customer3 }.AsQueryable());

            _customerRoleRepo = MockRepository.GenerateMock<IRepository<CustomerRole>>();
            _customerAttributeRepo = MockRepository.GenerateMock<IRepository<CustomerAttribute>>();
            _customerAttributeRepo = MockRepository.GenerateMock<IRepository<CustomerAttribute>>();
            newsLetterSubscriptionService = MockRepository.GenerateMock<INewsLetterSubscriptionService>();
            _customerService = new CustomerService(new NopNullCache(), _customerRepo, _customerRoleRepo,
                _customerAttributeRepo, _encryptionService, newsLetterSubscriptionService,
                _rewardPointsSettings, _customerSettings);
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
        public void Can_validate_a_hashed_password() 
        {
            bool result = _customerService.ValidateCustomer("a@b.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_a_clear_password() 
        {
            bool result = _customerService.ValidateCustomer("test@test.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_an_encrypted_password() 
        {
            bool result = _customerService.ValidateCustomer("user@test.com", "password");
            result.ShouldBeTrue();
        }

        private CustomerRegistrationRequest CreateCustomerRegistrationRequest(Customer customer) 
        {
            return new CustomerRegistrationRequest(customer, "test.user@domain.com", "test.user@domain.com", 
                "password", PasswordFormat.Encrypted);
        }
    }
}
