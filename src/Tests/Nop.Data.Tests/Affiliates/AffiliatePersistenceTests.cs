using System;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Affiliates
{
    [TestFixture]
    public class AffiliatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_affiliate()
        {
            var affiliate = new Affiliate
            {
                Deleted = true,
                Active = true,
                Address = GetTestAddress(),
                AdminComment = "AdminComment 1",
                FriendlyUrlName = "FriendlyUrlName 1"
            };

            var fromDb = SaveAndLoadEntity(affiliate);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(true);
            fromDb.Active.ShouldEqual(true);
            fromDb.Address.ShouldNotBeNull();
            fromDb.Address.FirstName.ShouldEqual("FirstName 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.FriendlyUrlName.ShouldEqual("FriendlyUrlName 1");
        }

        protected Address GetTestAddress()
        {
            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1a",
                Address2 = "Address1a",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = new Country
                {
                    Name = "United States",
                    TwoLetterIsoCode = "US",
                    ThreeLetterIsoCode = "USA",
                }
            };
        }
    }
}
