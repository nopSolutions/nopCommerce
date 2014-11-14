using System;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Catalog
{
    [TestFixture]
    public class ProductExtensionTests
    {
        [Test]
        public void Can_parse_required_product_ids()
        {
            var product = new Product
            {
                RequiredProductIds = "1, 4,7 ,a,"
            };

            var ids = product.ParseRequiredProductIds();
            ids.Length.ShouldEqual(3);
            ids[0].ShouldEqual(1);
            ids[1].ShouldEqual(4);
            ids[2].ShouldEqual(7);
        }

        [Test]
        public void Should_be_available_when_startdate_is_not_set()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = null
            };

            product.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_startdate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            product.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_startdate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
            };

            product.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(false);
        }

        [Test]
        public void Should_be_available_when_enddate_is_not_set()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = null
            };

            product.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(true);
        }

        [Test]
        public void Should_be_available_when_enddate_is_greater_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            product.IsAvailable(new DateTime(2010, 01, 01)).ShouldEqual(true);
        }

        [Test]
        public void Should_not_be_available_when_enddate_is_less_than_somedate()
        {
            var product = new Product
            {
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
            };

            product.IsAvailable(new DateTime(2010, 01, 03)).ShouldEqual(false);
        }
    }
}
