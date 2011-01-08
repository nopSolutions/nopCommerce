using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class ManufacturerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_manufacturer()
        {
            var manufacturer = new Manufacturer
            {
                Name = "Name",
                Description = "Description 1",
                TemplateId = 1,
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
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(manufacturer);
            fromDb.Name.ShouldEqual("Name");
            fromDb.Description.ShouldEqual("Description 1");
            fromDb.TemplateId.ShouldEqual(1);
            fromDb.MetaKeywords.ShouldEqual("Meta keywords");
            fromDb.MetaDescription.ShouldEqual("Meta description");
            fromDb.SeName.ShouldEqual("SE name");
            fromDb.PictureId.ShouldEqual(3);
            fromDb.PageSize.ShouldEqual(4);
            fromDb.PriceRanges.ShouldEqual("1-3;");
            fromDb.Published.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.DisplayOrder.ShouldEqual(5);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.UpdatedOnUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_manufacturer_with_productManufacturers()
        {
            var manufacturer = new Manufacturer
            {
                Name = "Name",
                Description = "Description 1",
                TemplateId = 1,
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
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                ProductManufacturers = new List<ProductManufacturer>()
                {
                    new ProductManufacturer
                    {
                        IsFeaturedProduct = true,
                        DisplayOrder = 1,
                        Product = new Product()
                        {
                            Name = "Name 1",
                            ShortDescription = "ShortDescription 1",
                            FullDescription = "FullDescription 1",
                            AdminComment = "AdminComment 1",
                            TemplateId = 1,
                            ShowOnHomePage = false,
                            MetaKeywords = "Meta keywords",
                            MetaDescription = "Meta description",
                            MetaTitle = "Meta title",
                            SeName = "SE name",
                            AllowCustomerReviews = true, 
                            AllowCustomerRatings = true,
                            RatingSum = 2,
                            TotalRatingVotes = 3,
                            Published = true,
                            Deleted = false,
                            CreatedOnUtc = new DateTime(2010, 01, 01),
                            UpdatedOnUtc = new DateTime(2010, 01, 02)
                        }
                    }
                }
            };

            var fromDb = SaveAndLoadEntity(manufacturer);
            fromDb.Name.ShouldEqual("Name");

            fromDb.ProductManufacturers.ShouldNotBeNull();
            (fromDb.ProductManufacturers.Count == 1).ShouldBeTrue();
            fromDb.ProductManufacturers.First().IsFeaturedProduct.ShouldEqual(true);
        }

        //[Test]
        //public void Can_save_and_load_manufacturer_with_localizedManufacturers()
        //{
        //    var lang = new Language()
        //    {
        //        Name = "English",
        //        LanguageCulture = "en-Us",
        //        FlagImageFileName = "us.png",
        //        Published = true,
        //        DisplayOrder = 1
        //    };

        //    var manufacturer = new Manufacturer
        //    {
        //        Name = "Name",
        //        Description = "Description 1",
        //        TemplateId = 1,
        //        MetaKeywords = "Meta keywords",
        //        MetaDescription = "Meta description",
        //        MetaTitle = "Meta title",
        //        SeName = "SE name",
        //        PictureId = 3,
        //        PageSize = 4,
        //        PriceRanges = "1-3;",
        //        Published = true,
        //        Deleted = false,
        //        DisplayOrder = 5,
        //        CreatedOnUtc = new DateTime(2010, 01, 01),
        //        UpdatedOnUtc = new DateTime(2010, 01, 02),

        //        LocalizedManufacturers = new List<LocalizedManufacturer>()
        //                                                     {
        //                                                         new LocalizedManufacturer
        //                                                             {
        //                                                                 Name = "Name localized 1",
        //                                                                 Description = "Description 1 localized",
        //                                                                 MetaKeywords = "Meta keywords localized",
        //                                                                 MetaDescription = "Meta description localized",
        //                                                                 MetaTitle = "Meta title localized",
        //                                                                 SeName = "SE name localized",
        //                                                                 Language = lang
        //                                                             },
        //                                                         new LocalizedManufacturer
        //                                                             {
        //                                                                 Name = "Name localized 2",
        //                                                                 Description = "Description 2 localized",
        //                                                                 MetaKeywords = "Meta keywords localized",
        //                                                                 MetaDescription = "Meta description localized",
        //                                                                 MetaTitle = "Meta title localized",
        //                                                                 SeName = "SE name localized",
        //                                                                 Language = lang
        //                                                             },
        //                                                         new LocalizedManufacturer
        //                                                             {
        //                                                                 Name = "Name localized 2",
        //                                                                 Description = "Description 2 localized",
        //                                                                 MetaKeywords = "Meta keywords localized",
        //                                                                 MetaDescription = "Meta description localized",
        //                                                                 MetaTitle = "Meta title localized",
        //                                                                 SeName = "SE name localized",
        //                                                                 Language = new Language()
        //                                                                                {
        //                                                                                    Name = "English 2",
        //                                                                                    LanguageCulture = "en-Us",
        //                                                                                    FlagImageFileName = "us.png",
        //                                                                                    Published = true,
        //                                                                                    DisplayOrder = 2
        //                                                                                }
        //                                                             },
        //                                                     }
        //    };

        //    var fromDb = SaveAndLoadEntity(manufacturer);
        //    fromDb.Name.ShouldEqual("Name");

        //    fromDb.LocalizedManufacturers.ShouldNotBeNull();
        //    (fromDb.LocalizedManufacturers.Count == 3).ShouldBeTrue();

        //}
    }
}
