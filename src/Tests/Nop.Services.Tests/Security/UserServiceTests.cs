using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Nop.Tests;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Security;

namespace Nop.Services.Tests.Security
{
    [TestFixture]
    public class UserServiceTests
    {
        IRepository<User> _userRepo;
        IEncryptionService _encryptionService;
        UserSettings _userSettings;
        SecuritySettings _securitySettings;

        [SetUp]
        public void SetUp() 
        {
            _userSettings = new UserSettings();
            _securitySettings = new SecuritySettings()
            {
                EncryptionKey = "273ece6f97dd844d"
            };
            _encryptionService = new EncryptionService(_securitySettings);
            _userRepo = MockRepository.GenerateMock<IRepository<User>>();
            var user = new User() 
            {
                Username = "a@b.com",
                Email = "a@b.com",
                PasswordFormat = PasswordFormat.Hashed,
                IsApproved = true
            };

            string saltKey = _encryptionService.CreateSaltKey(5);
            string password = _encryptionService.CreatePasswordHash("password", saltKey);
            user.PasswordSalt = saltKey;
            user.Password = password;

            var user2 = new User() 
            {
                Username = "test@test.com",
                Email = "test@test.com",
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                IsApproved = true
            };

            var user3 = new User() 
            {
                Username = "user@test.com",
                Email = "user@test.com",
                PasswordFormat = PasswordFormat.Encrypted,
                Password = _encryptionService.EncryptText("password"),
                IsApproved = true
            };

            _userRepo.Expect(x => x.Table).Return(new List<User>() { user, user2, user3 }.AsQueryable());
        }

        [Test]
        public void Can_register_a_user() 
        {
            var registrationRequest = CreateUserRegistrationRequest();
            var userService = new UserService(_encryptionService, _userRepo, _userSettings);
            var result = userService.RegisterUser(registrationRequest);

            result.Success.ShouldBeTrue();
            result.User.Username.ShouldEqual("test.user@domain.com");
        }

        [Test]
        public void Can_not_have_duplicate_usernames_or_emails() 
        {
            var registrationRequest = CreateUserRegistrationRequest();
            registrationRequest.Username = "a@b.com";
            registrationRequest.Email = "a@b.com";

            var userService = new UserService(_encryptionService, _userRepo, _userSettings);
            var result = userService.RegisterUser(registrationRequest);

            result.Success.ShouldBeFalse();
            result.Errors.Count.ShouldEqual(1);
        }

        [Test]
        public void Can_validate_a_hashed_password() 
        {
            var userService = new UserService(_encryptionService, _userRepo, _userSettings);
            bool result = userService.ValidateUser("a@b.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_a_clear_password() 
        {
            var userService = new UserService(_encryptionService, _userRepo, _userSettings);

            bool result = userService.ValidateUser("test@test.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_an_encrypted_password() 
        {
            var userService = new UserService(_encryptionService, _userRepo, _userSettings);

            bool result = userService.ValidateUser("user@test.com", "password");
            result.ShouldBeTrue();
        }

        private UserRegistrationRequest CreateUserRegistrationRequest() 
        {
            return new UserRegistrationRequest("test.user@domain.com", "test.user@domain.com", 
                "password", PasswordFormat.Encrypted);
        }
    }
}
