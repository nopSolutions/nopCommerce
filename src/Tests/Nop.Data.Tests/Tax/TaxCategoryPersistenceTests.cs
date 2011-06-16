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

namespace Nop.Data.Tests.Tax
{
    [TestFixture]
    public class TaxCategoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_taxCategory()
        {
            var taxCategory = new TaxCategory
                               {
                                   Name = "Books",
                                   DisplayOrder = 1
                               };

            var fromDb = SaveAndLoadEntity(taxCategory);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Books");
            fromDb.DisplayOrder.ShouldEqual(1);
        }
    }
}