using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.News
{
    [TestFixture]
    public class NewsPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_newsItem()
        {
            var news = new NewsItem
            {
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                Published = true,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                AllowComments = true,
                CommentCount = 1,
                LimitedToStores = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1",
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                }
            };

            var fromDb = SaveAndLoadEntity(news);
            fromDb.ShouldNotBeNull();
            fromDb.Title.ShouldEqual("Title 1");
            fromDb.Short.ShouldEqual("Short 1");
            fromDb.Full.ShouldEqual("Full 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
            fromDb.AllowComments.ShouldEqual(true);
            fromDb.CommentCount.ShouldEqual(1);
            fromDb.LimitedToStores.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 03));
            fromDb.MetaTitle.ShouldEqual("MetaTitle 1");
            fromDb.MetaDescription.ShouldEqual("MetaDescription 1");
            fromDb.MetaKeywords.ShouldEqual("MetaKeywords 1");

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }

        [Test]
        public void Can_save_and_load_newsItem_with_comments()
        {
            var news = new NewsItem
            {
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                AllowComments = true,
                Published = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                }
            };
            news.NewsComments.Add
                (
                    new NewsComment
                    {
                        CommentText = "Comment text 1",
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                        Customer = GetTestCustomer()
                    }
                );
            var fromDb = SaveAndLoadEntity(news);
            fromDb.ShouldNotBeNull();


            fromDb.NewsComments.ShouldNotBeNull();
            (fromDb.NewsComments.Count == 1).ShouldBeTrue();
            fromDb.NewsComments.First().CommentText.ShouldEqual("Comment text 1");
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}
