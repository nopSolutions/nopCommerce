using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class TierPriceExtensionTests
    {
        [Test]
        public void CanRemoveDuplicatedQuantities()
        {
            var tierPrices = new List<TierPrice>
            {
                new TierPrice
                {
                    //will be removed
                    Id = 1,
                    Price = 150,
                    Quantity = 1
                },
                new TierPrice
                {
                    //will stay
                    Id = 2,
                    Price = 100,
                    Quantity = 1
                },
                new TierPrice
                {
                    //will stay
                    Id = 3,
                    Price = 200,
                    Quantity = 3
                },
                new TierPrice
                {
                    //will stay
                    Id = 4,
                    Price = 250,
                    Quantity = 4
                },
                new TierPrice
                {
                    //will be removed
                    Id = 5,
                    Price = 300,
                    Quantity = 4
                },
                new TierPrice
                {
                    //will stay
                    Id = 6,
                    Price = 350,
                    Quantity = 5
                }
            };

            tierPrices = tierPrices.RemoveDuplicatedQuantities().ToList();

            tierPrices.FirstOrDefault(x => x.Id == 1).Should().BeNull();
            tierPrices.FirstOrDefault(x => x.Id == 2).Should().NotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 3).Should().NotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 4).Should().NotBeNull();
            tierPrices.FirstOrDefault(x => x.Id == 5).Should().BeNull();
            tierPrices.FirstOrDefault(x => x.Id == 6).Should().NotBeNull();
        }
    }
}
