using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Catalog
{
    [TestFixture]
    public class ProductVariantTests
    {
        [Test]
        public void Can_parse_required_productvariant_ids()
        {
            var productVariant = new ProductVariant
            {
                RequiredProductVariantIds = "1, 4,7 ,a,"
            };

            var ids = productVariant.ParseRequiredProductVariantIds();
            ids.Length.ShouldEqual(3);
            ids[0].ShouldEqual(1);
            ids[1].ShouldEqual(4);
            ids[2].ShouldEqual(7);
        }
    }
}
