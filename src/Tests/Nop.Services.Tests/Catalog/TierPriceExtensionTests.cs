using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class TierPriceExtensionTests : ServiceTest
    {
        [SetUp]
        public new void SetUp()
        {

        }

        [Test]
        public void Can_remove_duplicatedQuantities()
        {
            var tierPrices = new List<TierPrice>();
            tierPrices.Add(new TierPrice()
            {
                //will be removed
                Id = 1,
                Price = 150,
                Quantity = 1
            });
            tierPrices.Add(new TierPrice()
            {
                //will stay
                Id = 2,
                Price = 100,
                Quantity = 1
            });
            tierPrices.Add(new TierPrice()
            {
                //will stay
                Id = 3,
                Price = 200,
                Quantity = 3
            });
            tierPrices.Add(new TierPrice()
            {
                //will stay
                Id = 4,
                Price = 250,
                Quantity = 4
            });
            tierPrices.Add(new TierPrice()
            {
                //will be removed
                Id = 5,
                Price = 300,
                Quantity = 4
            });
            tierPrices.Add(new TierPrice()
            {
                //will stay
                Id = 6,
                Price = 350,
                Quantity = 5
            });

            tierPrices.RemoveDuplicatedQuantities();

            tierPrices.Where(x => x.Id == 1).FirstOrDefault().ShouldBeNull();
            tierPrices.Where(x => x.Id == 2).FirstOrDefault().ShouldNotBeNull();
            tierPrices.Where(x => x.Id == 3).FirstOrDefault().ShouldNotBeNull();
            tierPrices.Where(x => x.Id == 4).FirstOrDefault().ShouldNotBeNull();
            tierPrices.Where(x => x.Id == 5).FirstOrDefault().ShouldBeNull();
            tierPrices.Where(x => x.Id == 6).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
