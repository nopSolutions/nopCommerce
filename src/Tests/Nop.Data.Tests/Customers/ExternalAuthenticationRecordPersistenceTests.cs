using System;
using Nop.Core.Domain.Customers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class ExternalAuthenticationRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_externalAuthenticationRecord()
        {
            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                Email = "Email 1",
                ExternalIdentifier = "ExternalIdentifier 1",
                ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                OAuthToken = "OAuthToken 1",
                OAuthAccessToken = "OAuthAccessToken 1",
                ProviderSystemName = "ProviderSystemName 1",
                Customer = GetTestCustomer()
            };

            var fromDb = SaveAndLoadEntity(externalAuthenticationRecord);
            fromDb.ShouldNotBeNull();
            fromDb.Email.ShouldEqual("Email 1");
            fromDb.ExternalIdentifier.ShouldEqual("ExternalIdentifier 1");
            fromDb.ExternalDisplayIdentifier.ShouldEqual("ExternalDisplayIdentifier 1");
            fromDb.OAuthToken.ShouldEqual("OAuthToken 1");
            fromDb.OAuthAccessToken.ShouldEqual("OAuthAccessToken 1");
            fromDb.ProviderSystemName.ShouldEqual("ProviderSystemName 1");

            fromDb.Customer.ShouldNotBeNull();
        }
        
        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}