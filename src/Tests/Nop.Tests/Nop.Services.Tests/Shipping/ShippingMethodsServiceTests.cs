using FluentAssertions;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingMethodsServiceTests : ServiceTest<ShippingMethod>
{
    private IShippingMethodsService _shippingMethodsService;
    private ShippingMethodCountryMapping _shippingMethodCountryMapping;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _shippingMethodCountryMapping = new ShippingMethodCountryMapping { CountryId = 1, ShippingMethodId = 1 };

        _shippingMethodsService = GetService<IShippingMethodsService>();
        await _shippingMethodsService.InsertShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _shippingMethodsService.DeleteShippingMethodCountryMappingAsync(_shippingMethodCountryMapping);
    }

    [Test]
    public async Task CanGetAllShippingMethods()
    {
        var methods = await _shippingMethodsService.GetAllShippingMethodsAsync();

        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(3);

        methods = await _shippingMethodsService.GetAllShippingMethodsAsync(1);
        methods.Any().Should().BeTrue();
        methods.Count.Should().Be(2);
    }

    [Test]
    public async Task CanGetShippingMethodCountryMapping()
    {
        var mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 1);
        mapping.Any().Should().BeTrue();

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(1, 2);
        mapping.Any().Should().BeFalse();

        mapping = await _shippingMethodsService.GetShippingMethodCountryMappingAsync(2, 1);
        mapping.Any().Should().BeFalse();
    }

    [Test]
    public async Task CanCheckCountryRestrictionExists()
    {
        var isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 1);
        isExists.Should().BeTrue();

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(1), 2);
        isExists.Should().BeFalse();

        isExists = await _shippingMethodsService.CountryRestrictionExistsAsync(await _shippingMethodsService.GetShippingMethodByIdAsync(2), 1);
        isExists.Should().BeFalse();
    }

    protected override CrudData<ShippingMethod> CrudData
    {
        get
        {
            var baseEntity = new ShippingMethod
            {
                Name = "Test shipping method",
                Description = "Test shipping method description",
                DisplayOrder = 1
            };

            var updatedEntity = new ShippingMethod
            {
                Name = "Test shipping method",
                Description = "Test shipping method description",
                DisplayOrder = 2
            };

            return new CrudData<ShippingMethod>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _shippingMethodsService.InsertShippingMethodAsync,
                Update = _shippingMethodsService.UpdateShippingMethodAsync,
                Delete = _shippingMethodsService.DeleteShippingMethodAsync,
                GetById = _shippingMethodsService.GetShippingMethodByIdAsync,
                IsEqual = (first, second) => first.DisplayOrder == second.DisplayOrder && first.Name.Equals(second.Name) && first.Description.Equals(second.Description)
            };
        }
    }
}
