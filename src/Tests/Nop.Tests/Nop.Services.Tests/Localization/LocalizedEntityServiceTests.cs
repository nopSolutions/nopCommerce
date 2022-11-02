using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Localization
{
    [TestFixture]
    public class LocalizedEntityServiceTests : BaseNopTest
    {
        private ILocalizedEntityService _localizedEntityService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _localizedEntityService = GetService<ILocalizedEntityService>();
        }

        [Test]
        public async Task CanSaveLocalizedValueAsync()
        {
            var product = await GetService<IProductService>().GetProductByIdAsync(1);

            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.Name, "test lang 1", 1);
            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.BasepriceAmount, 1.0M, 1);

            var name = await _localizedEntityService.GetLocalizedValueAsync(1, 1, nameof(Product),
                nameof(Product.Name));

            name.Should().Be("test lang 1");

            var basePriceAmount = await _localizedEntityService.GetLocalizedValueAsync(1, 1, nameof(Product),
                nameof(Product.BasepriceAmount));

            decimal.Parse(basePriceAmount, CultureInfo.InvariantCulture).Should().Be(1M);

            basePriceAmount = await _localizedEntityService.GetLocalizedValueAsync(2, 1, nameof(Product),
                nameof(Product.BasepriceAmount));

            basePriceAmount.Should().BeNullOrEmpty();
        }
    }
}