using System;
using FluentAssertions;
using Nop.Core.Domain.News;
using Nop.Services.News;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.News
{
    [TestFixture]
    public class NewsServiceTests : ServiceTest
    {
        private INewsService _newsService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _newsService = GetService<INewsService>();
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsNotSet()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = null
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
        {
            var newsItem = new NewsItem
            {
                StartDateUtc = new DateTime(2010, 01, 02)
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 01)).Should().BeFalse();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsNotSet()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = null
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeTrue();
        }

        [Test]
        public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 01)).Should().BeTrue();
        }

        [Test]
        public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
        {
            var newsItem = new NewsItem
            {
                EndDateUtc = new DateTime(2010, 01, 02)
            };

            _newsService.IsNewsAvailable(newsItem, new DateTime(2010, 01, 03)).Should().BeFalse();
        }
    }
}
