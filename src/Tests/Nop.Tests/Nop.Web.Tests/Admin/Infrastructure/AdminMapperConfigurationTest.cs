using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Infrastructure;

[TestFixture]
public class AdminMapperConfigurationTest : BaseNopTest
{
    private IOrderService _orderService;
    private IProductService _productService;

    [SetUp]
    public void SetUp()
    {
        _orderService = GetService<IOrderService>();
        _productService = GetService<IProductService>();
    }

    [Test]
    public void ConfigurationIsValid()
    {
        var config = new List<IOrderedMapperProfile> { new AdminMapperConfiguration() };
        MapperConfiguration.Init(config.OrderBy(p => p.Order));

        Assert.DoesNotThrow(() => MapperConfiguration.TypeAdapterConfig.Compile());
    }

    [Test]
    [Ignore("Not a test, used for profiling.")]
    public async Task Profile()
    {
        var config = new List<IOrderedMapperProfile> { new AdminMapperConfiguration() };
        MapperConfiguration.Init(config.OrderBy(p => p.Order));
        MapperConfiguration.TypeAdapterConfig.Compile();

        var orders = await _orderService.SearchOrdersAsync();
        var products = await _productService.SearchProductsAsync();

        TestHelper.ProfileAction(() =>
        {
            Console.WriteLine("First call");
            _ = orders.First().ToModel<CustomerOrderModel>().ToEntity<Order>();
            _ = products.First().ToModel<ProductModel>().ToEntity<Product>();
        });

        TestHelper.ProfileAction(() =>
        {
            var orderModels = new List<CustomerOrderModel>();
            const int multiplicator = 1000;

            for (var i = 0; i <= multiplicator; i++)
                orderModels.AddRange(orders.Select(order => order.ToModel<CustomerOrderModel>()));

            var productModels = new List<ProductModel>();
            for (var i = 0; i <= multiplicator; i++)
                productModels.AddRange(products.Select(product => product.ToModel<ProductModel>()));

            foreach (var model in orderModels)
                _ = model.ToEntity<Order>();

            foreach (var model in productModels)
                _ = model.ToEntity<Product>();

            Console.WriteLine($"Total items: {(productModels.Count + orderModels.Count) *2 }");
        });
    }
}