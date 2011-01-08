using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Tests
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
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(currency);
            fromDb.Name.ShouldEqual("US Dollar");
            fromDb.CurrencyCode.ShouldEqual("USD");
            fromDb.Rate.ShouldEqual(1);
            fromDb.DisplayLocale.ShouldEqual("en-US");
            fromDb.CustomFormatting.ShouldEqual("CustomFormatting 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(2);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }
    }
}
