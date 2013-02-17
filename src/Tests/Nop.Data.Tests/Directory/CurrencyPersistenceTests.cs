using System;
using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
{
    [TestFixture]
    public class CurrencyPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_currency()
        {
            var currency = new Currency
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1.1M,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                LimitedToStores = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(currency);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("US Dollar");
            fromDb.CurrencyCode.ShouldEqual("USD");
            fromDb.Rate.ShouldEqual(1.1M);
            fromDb.DisplayLocale.ShouldEqual("en-US");
            fromDb.CustomFormatting.ShouldEqual("CustomFormatting 1");
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(2);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
