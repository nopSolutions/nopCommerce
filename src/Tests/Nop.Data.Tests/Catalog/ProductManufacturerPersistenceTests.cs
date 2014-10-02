using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productManufacturer()
        {
            var productManufacturer = new ProductManufacturer
                                     {
                                         IsFeaturedProduct = true,
                                         DisplayOrder = 1,
                                         Product = new Product
                                                       {
                                                           Name = "Name 1",
                                                           Published = true,
                                                           Deleted = false,
                                                           CreatedOnUtc = new DateTime(2010, 01, 01),
                                                           UpdatedOnUtc = new DateTime(2010, 01, 02)
                                                       },
                                         Manufacturer = new Manufacturer
                                                                      {
                                                                          Name = "Name",
                                                                          Description = "Description 1",
                                                                          MetaKeywords = "Meta keywords",
                                                                          MetaDescription = "Meta description",
                                                                          MetaTitle = "Meta title",
                                                                          PictureId = 3,
                                                                          PageSize = 4,
                                                                          PriceRanges = "1-3;",
                                                                          Published = true,
                                                                          Deleted = false,
                                                                          DisplayOrder = 5,
                                                                          CreatedOnUtc = new DateTime(2010, 01, 01),
                                                                          UpdatedOnUtc = new DateTime(2010, 01, 02),
                                                                      }
                                     };

            var fromDb = SaveAndLoadEntity(productManufacturer);
            fromDb.ShouldNotBeNull();
            fromDb.IsFeaturedProduct.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Manufacturer.ShouldNotBeNull();
            fromDb.Manufacturer.Name.ShouldEqual("Name");
        }
    }
}
