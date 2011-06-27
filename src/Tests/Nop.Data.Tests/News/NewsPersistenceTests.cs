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
                AllowComments = true,
                Published = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Language = new Language()
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
            fromDb.AllowComments.ShouldEqual(true);
            fromDb.Published.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

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
                Language = new Language()
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
                        IpAddress = "192.168.1.1",
                        IsApproved = true,
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                        UpdatedOnUtc = new DateTime(2010, 01, 04),
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
