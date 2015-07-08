using Nop.Core.Domain.Stores;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Stores
{
    [TestFixture]
    public class StorePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_store()
        {
            var store = new Store
            {
                Name = "Computer store",
                Url = "http://www.yourStore.com",
                Hosts = "yourStore.com,www.yourStore.com",
                DefaultLanguageId = 1,
                DisplayOrder = 2,
                CompanyName = "company name",
                CompanyAddress = "some address",
                CompanyPhoneNumber = "123456789",
                CompanyVat = "some vat",
            };

            var fromDb = SaveAndLoadEntity(store);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Computer store");
            fromDb.Url.ShouldEqual("http://www.yourStore.com");
            fromDb.Hosts.ShouldEqual("yourStore.com,www.yourStore.com");
            fromDb.DefaultLanguageId.ShouldEqual(1);
            fromDb.DisplayOrder.ShouldEqual(2);
            fromDb.CompanyName.ShouldEqual("company name");
            fromDb.CompanyAddress.ShouldEqual("some address");
            fromDb.CompanyPhoneNumber.ShouldEqual("123456789");
            fromDb.CompanyVat.ShouldEqual("some vat");
        }
    }
}
