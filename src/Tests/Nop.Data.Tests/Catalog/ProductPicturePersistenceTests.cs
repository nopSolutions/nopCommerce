using System;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPicturePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_productPicture()
        {
            var productPicture = new ProductPicture
                                     {
                                         DisplayOrder = 1,
                                         Product = new Product
                                                       {
                                                           Name = "Name 1",
                                                           Published = true,
                                                           Deleted = false,
                                                           CreatedOnUtc = new DateTime(2010, 01, 01),
                                                           UpdatedOnUtc = new DateTime(2010, 01, 02)
                                                       },
                                         Picture = new Picture
                                                                      {
                                                                          PictureBinary = new byte[] { 1, 2, 3 },
                                                                          MimeType = MimeTypes.ImagePJpeg,
                                                                          IsNew = true
                                                                      }
                                     };

            var fromDb = SaveAndLoadEntity(productPicture);
            fromDb.ShouldNotBeNull();
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Product.ShouldNotBeNull();
            fromDb.Product.Name.ShouldEqual("Name 1");

            fromDb.Picture.ShouldNotBeNull();
            fromDb.Picture.MimeType.ShouldEqual(MimeTypes.ImagePJpeg);
        }
    }
}
