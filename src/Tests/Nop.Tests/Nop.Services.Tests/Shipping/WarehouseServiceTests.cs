using FluentAssertions;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class WarehouseServiceTests : ServiceTest<Warehouse>
{
    private IAddressService _addressService;
    private IWarehouseService _warehouseService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _addressService = GetService<IAddressService>();
        _warehouseService = GetService<IWarehouseService>();
    }

    [Test]
    public async Task CanGetAllWarehouses()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        warehouses.Any().Should().BeTrue();
        warehouses.Count.Should().Be(2);

        warehouses = await _warehouseService.GetAllWarehousesAsync("Warehouse 1 (New York)");
        warehouses.Any().Should().BeTrue();
        warehouses.Count.Should().Be(1);
    }

    [Test]
    public async Task CanGetNearestWarehouse()
    {
        var address = await _addressService.GetAddressByIdAsync(1);

        var warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
        warehouses.Should().NotBeNull();
        warehouses.Name.Should().BeEquivalentTo("Warehouse 1 (New York)");

        address = await _addressService.GetAddressByIdAsync(2);

        warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
        warehouses.Should().NotBeNull();
        warehouses.Name.Should().BeEquivalentTo("Warehouse 2 (Los Angeles)");

        address = await _addressService.GetAddressByIdAsync(3);

        warehouses = await _warehouseService.GetNearestWarehouseAsync(address);
        warehouses.Should().NotBeNull();

        warehouses = await _warehouseService.GetNearestWarehouseAsync(null);
        warehouses.Should().NotBeNull();
    }

    protected override CrudData<Warehouse> CrudData
    {
        get
        {
            var baseEntity = new Warehouse
            {
                Name = "Test shipping method",
                AddressId = 1,
                AdminComment = "Test admin comment"
            };

            var updatedEntity = new Warehouse
            {
                Name = "New test shipping method",
                AddressId = 1,
                AdminComment = "Test admin comment"
            };

            return new CrudData<Warehouse>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _warehouseService.InsertWarehouseAsync,
                Update = _warehouseService.UpdateWarehouseAsync,
                Delete = _warehouseService.DeleteWarehouseAsync,
                GetById = _warehouseService.GetWarehouseByIdAsync,
                IsEqual = (first, second) => first.AddressId == second.AddressId && first.Name.Equals(second.Name) && first.AdminComment.Equals(second.AdminComment)
            };
        }
    }
}
