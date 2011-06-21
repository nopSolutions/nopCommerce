using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Security;
using NUnit.Framework;
using Nop.Tests;
using Nop.Services.Security;

namespace Nop.Services.Tests.Security
{
    [TestFixture]
    public class EncryptionServiceTests : ServiceTest
    {
        IEncryptionService _encryptionService;
        SecuritySettings _securitySettings;

        [SetUp]
        public void SetUp() 
        {
            _securitySettings = new SecuritySettings()
            {
                EncryptionKey = "273ece6f97dd844d"
            };
            _encryptionService = new EncryptionService(_securitySettings);
        }

        [Test]
        public void Can_hash() 
        {
            string password = "MyLittleSecret";
            var saltKey = _encryptionService.CreateSaltKey(5);
            var hashedPassword = _encryptionService.CreatePasswordHash(password, saltKey);
            hashedPassword.ShouldBeNotBeTheSameAs(password);
        }

        [Test]
        public void Can_encrypt_and_decrypt() 
        {
            var password = "MyLittleSecret";
            string encryptedPassword = _encryptionService.EncryptText(password);
            var decryptedPassword = _encryptionService.DecryptText(encryptedPassword);
            decryptedPassword.ShouldEqual(password);
        }
    }
}
