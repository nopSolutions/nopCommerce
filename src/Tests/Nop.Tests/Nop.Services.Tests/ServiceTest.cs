using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data.Configuration;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Tests.Nop.Services.Tests.Directory;
using Nop.Tests.Nop.Services.Tests.Discounts;
using Nop.Tests.Nop.Services.Tests.Payments;
using Nop.Tests.Nop.Services.Tests.Shipping;
using Nop.Tests.Nop.Services.Tests.Tax;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests;

[TestFixture]
public abstract class ServiceTest : BaseNopTest
{
    protected ServiceTest()
    {
        //init plugins
        InitPlugins();
    }

    private static void InitPlugins()
    {
        var webHostEnvironment = new Mock<IWebHostEnvironment>();
        webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
        webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
        CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);

        Environment.SetEnvironmentVariable("ConnectionStrings", Singleton<DataConfig>.Instance.ConnectionString);

        Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
        {
            PluginDescriptors = new List<(PluginDescriptor, bool)>
            {
                (new PluginDescriptor
                {
                    PluginType = typeof(FixedRateTestTaxProvider),
                    SystemName = "FixedTaxRateTest",
                    FriendlyName = "Fixed tax test rate provider",
                    Installed = true,
                    ReferencedAssembly = typeof(FixedRateTestTaxProvider).Assembly
                }, true),
                (new PluginDescriptor
                {
                    PluginType = typeof(FixedRateTestShippingRateComputationMethod),
                    SystemName = "FixedRateTestShippingRateComputationMethod",
                    FriendlyName = "Fixed rate test shipping computation method",
                    Installed = true,
                    ReferencedAssembly = typeof(FixedRateTestShippingRateComputationMethod).Assembly
                }, true),
                (new PluginDescriptor
                {
                    PluginType = typeof(TestPaymentMethod),
                    SystemName = "Payments.TestMethod",
                    FriendlyName = "Test payment method",
                    Installed = true,
                    ReferencedAssembly = typeof(TestPaymentMethod).Assembly
                }, true),
                (new PluginDescriptor
                {
                    PluginType = typeof(TestDiscountRequirementRule),
                    SystemName = "TestDiscountRequirementRule",
                    FriendlyName = "Test discount requirement rule",
                    Installed = true,
                    ReferencedAssembly = typeof(TestDiscountRequirementRule).Assembly
                }, true),
                (new PluginDescriptor
                {
                    PluginType = typeof(TestExchangeRateProvider),
                    SystemName = "CurrencyExchange.TestProvider",
                    FriendlyName = "Test exchange rate provider",
                    Installed = true,
                    ReferencedAssembly = typeof(TestExchangeRateProvider).Assembly
                }, true),
                (new PluginDescriptor
                {
                    PluginType = typeof(PickupPointTestProvider),
                    SystemName = "PickupPoint.TestProvider",
                    FriendlyName = "Test pickup point provider",
                    Installed = true,
                    ReferencedAssembly = typeof(PickupPointTestProvider).Assembly
                }, true)
            }
        };

        var taxSettings = GetService<TaxSettings>();
        var settingsService = GetService<ISettingService>();

        taxSettings.ActiveTaxProviderSystemName = "FixedTaxRateTest";
        settingsService.SaveSetting(taxSettings, settings => settings.ActiveTaxProviderSystemName);

        var shippingSettings = GetService<ShippingSettings>();
        shippingSettings.ActivePickupPointProviderSystemNames.Add("PickupPoint.TestProvider");
        settingsService.SaveSetting(shippingSettings, settings => settings.ActivePickupPointProviderSystemNames);
    }
}

public abstract class ServiceTest<TEntity> : ServiceTest where TEntity : BaseEntity
{
    protected abstract CrudData<TEntity> CrudData { get; }

    [Test]
    public async Task TestCrudAsync()
    {
        var data = CrudData;

        data.BaseEntity.Id = 0;

        await data.Insert(data.BaseEntity);
        data.BaseEntity.Id.Should().BeGreaterThan(0);

        data.UpdatedEntity.Id = data.BaseEntity.Id;
        await data.Update(data.UpdatedEntity);

        var item = await data.GetById(data.BaseEntity.Id);
        item.Should().NotBeNull();
        data.IsEqual(data.UpdatedEntity, item).Should().BeTrue();

        await data.Delete(data.BaseEntity);
        item = await data.GetById(data.BaseEntity.Id);

        if (data.BaseEntity is ISoftDeletedEntity softDeletedEntity)
            softDeletedEntity.Deleted.Should().BeTrue();
        else
            item.Should().BeNull();
    }
}