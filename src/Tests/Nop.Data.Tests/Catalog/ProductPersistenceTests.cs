using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Catalog
{
    [TestFixture]
    public class ProductPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_product()
        {
            var product = new Product
            {
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                ProductTemplateId = 1,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SeName = "SE name",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.ShortDescription.ShouldEqual("ShortDescription 1");
            fromDb.FullDescription.ShouldEqual("FullDescription 1");
            fromDb.AdminComment.ShouldEqual("AdminComment 1");
            fromDb.ProductTemplateId.ShouldEqual(1);
            fromDb.ShowOnHomePage.ShouldEqual(false);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.SeName.ShouldEqual("SE name");
            fromDb.AllowCustomerReviews.ShouldEqual(true);
            fromDb.ApprovedRatingSum.ShouldEqual(2);
            fromDb.NotApprovedRatingSum.ShouldEqual(3);
            fromDb.ApprovedTotalReviews.ShouldEqual(4);
            fromDb.NotApprovedTotalReviews.ShouldEqual(5);
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_product_with_productVariants()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductVariants.Add
                (
                    new ProductVariant
                    {
                        Name = "Product variant name 1",
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                        UpdatedOnUtc = new DateTime(2010, 01, 04)
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.ProductVariants.ShouldNotBeNull();
            (fromDb.ProductVariants.Count == 1).ShouldBeTrue();
            fromDb.ProductVariants.First().Name.ShouldEqual("Product variant name 1");
        }

        [Test]
        public void Can_save_and_load_product_with_productCategories()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductCategories.Add
                (
                    new ProductCategory
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Category = new Category()
                        {
                            Name = "Books",
                            Description = "Description 1",
                            MetaKeywords = "Meta keywords",
                            MetaDescription = "Meta description",
                            MetaTitle = "Meta title",
                            SeName = "SE name",
                            ParentCategoryId = 2,
                            PictureId = 3,
                            PageSize = 4,
                            PriceRanges = "1-3;",
                            ShowOnHomePage = false,
                            Published = true,
                            Deleted = false,
                            DisplayOrder = 5,
                            CreatedOnUtc = new DateTime(2010, 01, 01),
                            UpdatedOnUtc = new DateTime(2010, 01, 02),
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductCategories.ShouldNotBeNull();
            (fromDb.ProductCategories.Count == 1).ShouldBeTrue();
            fromDb.ProductCategories.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductCategories.First().Category.ShouldNotBeNull();
            fromDb.ProductCategories.First().Category.Name.ShouldEqual("Books");
        }

        [Test]
        public void Can_save_and_load_product_with_productManufacturers()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductManufacturers.Add
                (
                    new ProductManufacturer
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Manufacturer = new Manufacturer()
                        {
                            Name = "Name",
                            Description = "Description 1",
                            MetaKeywords = "Meta keywords",
                            MetaDescription = "Meta description",
                            MetaTitle = "Meta title",
                            SeName = "SE name",
                            PictureId = 3,
                            PageSize = 4,
                            PriceRanges = "1-3;",
                            Published = true,
                            Deleted = false,
                            DisplayOrder = 5,
                            CreatedOnUtc =
                                new DateTime(2010, 01, 01),
                            UpdatedOnUtc =
                                new DateTime(2010, 01, 02),
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductManufacturers.ShouldNotBeNull();
            (fromDb.ProductManufacturers.Count == 1).ShouldBeTrue();
            fromDb.ProductManufacturers.First().IsFeaturedProduct.ShouldEqual(true);

            fromDb.ProductManufacturers.First().Manufacturer.ShouldNotBeNull();
            fromDb.ProductManufacturers.First().Manufacturer.Name.ShouldEqual("Name");
        }

        [Test]
        public void Can_save_and_load_product_with_productPictures()
        {
            var product = new Product
                          {
                              Name = "Name 1",
                              Published = true,
                              Deleted = false,
                              CreatedOnUtc = new DateTime(2010, 01, 01),
                              UpdatedOnUtc = new DateTime(2010, 01, 02)
                          };
            product.ProductPictures.Add
                (
                    new ProductPicture
                    {
                        DisplayOrder = 1,
                        Picture = new Picture()
                        {
                            PictureBinary = new byte[] { 1, 2, 3 },
                            MimeType = "image/pjpeg",
                            IsNew = true
                        }
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");

            fromDb.ProductPictures.ShouldNotBeNull();
            (fromDb.ProductPictures.Count == 1).ShouldBeTrue();
            fromDb.ProductPictures.First().DisplayOrder.ShouldEqual(1);

            fromDb.ProductPictures.First().Picture.ShouldNotBeNull();
            fromDb.ProductPictures.First().Picture.MimeType.ShouldEqual("image/pjpeg");
        }

        [Test]
        public void Can_save_and_load_product_with_productTags()
        {
            var product = new Product
            {
                Name = "Name 1",
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
            product.ProductTags.Add
                (
                    new ProductTag
                    {
                        Name = "Tag name 1",
                        ProductCount = 1
                    }
                );
            var fromDb = SaveAndLoadEntity(product);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");


            fromDb.ProductTags.ShouldNotBeNull();
            (fromDb.ProductTags.Count == 1).ShouldBeTrue();
            fromDb.ProductTags.First().Name.ShouldEqual("Tag name 1");
        }

    }
}
