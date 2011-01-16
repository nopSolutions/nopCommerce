using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core.Domain.Tax;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class TaxProviderPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_taxProvider()
        {
            var taxProvider = new TaxProvider
                               {
                                   Name = "Fixed tax rate provider",
                                   Description = "Description 1",
                                   ClassName = "Nop.Tax.FixedRateTaxProvider.FixedRateTaxProvider, Nop.Tax.FixedRateTaxProvider",
                                   DisplayOrder = 1
                               };

            var fromDb = SaveAndLoadEntity(taxProvider);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Fixed tax rate provider");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.ClassName.ShouldEqual("Nop.Tax.FixedRateTaxProvider.FixedRateTaxProvider, Nop.Tax.FixedRateTaxProvider");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}