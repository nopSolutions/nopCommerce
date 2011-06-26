using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductTagPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productTag()
        {
            var productTag = new ProductTag
                               {
                                   Name = "Name 1",
                                   ProductCount = 1,
                               };

            var fromDb = SaveAndLoadEntity(productTag);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ProductCount.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_productTag_with_products()
        {
            var productTag = new ProductTag
            {
                Name = "Name 1",
                ProductCount = 1,
            };
            productTag.Products.Add(GetTestProduct());

            var fromDb = SaveAndLoadEntity(productTag);
            fromDb.ShouldNotBeNull();


            fromDb.Products.ShouldNotBeNull();
            (fromDb.Products.Count == 1).ShouldBeTrue();
            fromDb.Products.First().Name.ShouldEqual("Name 1");
        }

        protected Country GetTestCountry()
        {
            return new Country
                {
                    Name = "United States",
                    TwoLetterIsoCode = "US",
                    ThreeLetterIsoCode = "USA",
                };
        }
        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Name 1",
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }
    }
}