using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        IRepository<User> userRepo;
        IEncryptionService encryptionService;
        UserSettings userSettings;
        string encryptionKey;

        [SetUp]
        public void SetUp() 
        {
            userSettings = new UserSettings();

            encryptionService = new EncryptionService();
            userRepo = MockRepository.GenerateMock<IRepository<User>>();
            encryptionKey = "273ece6f97dd844d";
            var user = new User() {
                Username = "a@b.com",
                Email = "a@b.com",
                PasswordFormat = PasswordFormat.Hashed,
            };

            string saltKey = encryptionService.CreateSaltKey(5);
            string password = encryptionService.CreatePasswordHash("password", saltKey);
            user.PasswordSalt = saltKey;
            user.Password = password;

            var user2 = new User() {
                Username = "test@test.com",
                Email = "test@test.com",
                PasswordFormat = PasswordFormat.Clear,
                Password = "password"
            };

            var user3 = new User() {
                Username = "user@test.com",
                Email = "user@test.com",
                PasswordFormat = PasswordFormat.Encrypted,
                Password = encryptionService.EncryptText("password", encryptionKey)
            };

            userRepo.Expect(x => x.Table).Return(new List<User>() { user, user2, user3 }.AsQueryable());
        }

        [Test]
        public void Can_register_a_user() {
            var registrationRequest = CreateUserRegistrationRequest();
            var userService = new UserService(encryptionService, userRepo, userSettings);
            var result = userService.RegisterUser(registrationRequest);

            result.Success.ShouldBeTrue();
            result.User.Username.ShouldEqual("test.user@domain.com");
        }

        [Test]
        public void Can_not_have_duplicate_usernames_or_emails() {
            var registrationRequest = CreateUserRegistrationRequest();
            registrationRequest.Username = "a@b.com";
            registrationRequest.Email = "a@b.com";

            var userService = new UserService(encryptionService, userRepo, userSettings);
            var result = userService.RegisterUser(registrationRequest);

            result.Success.ShouldBeFalse();
            result.Errors.Count.ShouldEqual(1);
        }

        [Test]
        public void Can_validate_a_hashed_password() {
            var userService = new UserService(encryptionService, userRepo, userSettings);
            bool result = userService.ValidateUser("a@b.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_a_clear_password() {
            var userService = new UserService(encryptionService, userRepo, userSettings);

            bool result = userService.ValidateUser("test@test.com", "password");
            result.ShouldBeTrue();
        }

        [Test]
        public void Can_validate_an_encrypted_password() {
            var userService = new UserService(encryptionService, userRepo, userSettings);

            bool result = userService.ValidateUser("user@test.com", "password");
            result.ShouldBeTrue();
        }

        private UserRegistrationRequest CreateUserRegistrationRequest() {
            return new UserRegistrationRequest("test.user@domain.com",
                "password", PasswordFormat.Encrypted, "", "", "test.user@domain.com", "Some question", "Some Answer");
        }
    }
}
