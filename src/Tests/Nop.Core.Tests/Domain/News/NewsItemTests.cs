using System;
using Nop.Core.Domain.News;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.News
{
    [TestFixture]
    public class NewsItemTest
    {        
        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = null
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(false);
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = null
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            newsItem.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(false);
        }
    }
}
