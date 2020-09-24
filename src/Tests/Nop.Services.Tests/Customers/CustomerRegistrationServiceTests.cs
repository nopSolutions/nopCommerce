using System;
using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Customers
{
    [TestFixture]
    public class CustomerRegistrationServiceTests : ServiceTest
    {
        private ICustomerService _customerService;
        private IEncryptionService _encryptionService;
        private ICustomerRegistrationService _customerRegistrationService;

        [SetUp]
        public void SetUp()
        {
            _customerService = GetService<ICustomerService>();
            _encryptionService = GetService<IEncryptionService>();
            _customerRegistrationService = GetService<ICustomerRegistrationService>();
        }

        private Customer CreateCustomer(PasswordFormat passwordFormat, bool isRegistered = true)
        {
            var customer = new Customer
            {
                Username = "test@test.com",
                Email = "test@test.com",
                Active = true
            };

            _customerService.InsertCustomer(customer);

            var password = "password";
            if (passwordFormat == PasswordFormat.Encrypted)
                password = _encryptionService.EncryptText(password);

            _customerService.InsertCustomerPassword(new CustomerPassword
            {
                CustomerId = customer.Id,
                PasswordFormat = passwordFormat,
                Password = password,
                CreatedOnUtc = DateTime.UtcNow
            });

            if (isRegistered)
                _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping
                {
                    CustomerId = customer.Id,
                    CustomerRoleId = _customerService
                        .GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName).Id
                });

            return customer;
        }

        private void DeleteCustomer(Customer customer)
        {
            customer.Username = customer.Email = string.Empty;
            customer.Active = false;
            _customerService.UpdateCustomer(customer);
            _customerService.DeleteCustomer(customer);
        }

        [Test]
        public void EnsureOnlyRegisteredCustomersCanLogin()
        {
            var result = _customerRegistrationService.ValidateCustomer(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword);
            result.Should().Be(CustomerLoginResults.Successful);

            var customer = CreateCustomer(PasswordFormat.Clear, false);

            result = _customerRegistrationService.ValidateCustomer("test@test.com", "password");
            DeleteCustomer(customer);

            result.Should().Be(CustomerLoginResults.NotRegistered);
        }

        [Test]
        public void CanValidateHashedPassword()
        {
            var result = _customerRegistrationService.ValidateCustomer(NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminPassword);
            result.Should().Be(CustomerLoginResults.Successful);
        }

        [Test]
        public void CanValidateClearPassword()
        {
            var customer = CreateCustomer(PasswordFormat.Clear);

            var result = _customerRegistrationService.ValidateCustomer("test@test.com", "password");
            DeleteCustomer(customer);

            result.Should().Be(CustomerLoginResults.Successful);
        }

        [Test]
        public void CanValidateEncryptedPassword()
        {
            var customer = CreateCustomer(PasswordFormat.Encrypted);

            var result = _customerRegistrationService.ValidateCustomer("test@test.com", "password");
            DeleteCustomer(customer);

            result.Should().Be(CustomerLoginResults.Successful);
        }
        
        [Test]
        public void CanChangePassword()
        {
            var customer = CreateCustomer(PasswordFormat.Encrypted);

            var request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Clear, "password", "password");
            var unSuccess = _customerRegistrationService.ChangePassword(request);
            
            request = new ChangePasswordRequest("test@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
            var success = _customerRegistrationService.ChangePassword(request);

            unSuccess.Success.Should().BeFalse();
            success.Success.Should().BeTrue();

            DeleteCustomer(customer);
        }
    }
}
