using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Services.Security;

namespace Nop.Services.Tests.Security
{
    [TestFixture]
    public class EncryptionServiceTests
    {
        IEncryptionService encryptionService;
        string encryptionKey;

        [SetUp]
        public void SetUp() {
            encryptionService = new EncryptionService();
            encryptionKey = "273ece6f97dd844d";
        }

        [Test]
        public void Can_hash() {
            string password = "MyLittleSecret";
            var saltKey = encryptionService.CreateSaltKey(5);
            var hashedPassword = encryptionService.CreatePasswordHash(password, saltKey);
            hashedPassword.ShouldBeNotBeTheSameAs(password);
        }

        [Test]
        public void Can_encrypt_and_decrypt() {
            var password = "MyLittleSecret";
            string encryptedPassword = encryptionService.EncryptText(password, encryptionKey);
            var decryptedPassword = encryptionService.DecryptText(encryptedPassword, encryptionKey);
            decryptedPassword.ShouldEqual(password);
        }
    }
}
