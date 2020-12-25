using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Tax;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests : ServiceTest
    {
        private TaxSettings _taxSettings;
        private bool _defaultEuVatAssumeValid;
        private ITaxPluginManager _taxPluginManager;
        private ISettingService _settingService;
        private ITaxService _taxService;
        private ICustomerService _customerService;
        private bool _defaultAdminRoleTaxExempt;
        private bool _defaultAdminTaxExempt;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _taxService = GetService<ITaxService>();
            _taxPluginManager = GetService<ITaxPluginManager>();
            _taxSettings = GetService<TaxSettings>();
            _settingService = GetService<ISettingService>();
            _customerService = GetService<ICustomerService>();

            _defaultEuVatAssumeValid = _taxSettings.EuVatAssumeValid;
            _taxSettings.EuVatAssumeValid = false;
            await _settingService.SaveSettingAsync(_taxSettings);

            var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
            _defaultAdminRoleTaxExempt = adminRole.TaxExempt;
            var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            _defaultAdminTaxExempt = admin.IsTaxExempt;
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _taxSettings.EuVatAssumeValid = _defaultEuVatAssumeValid;
            await _settingService.SaveSettingAsync(_taxSettings);

            var adminRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
            adminRole.TaxExempt = _defaultAdminRoleTaxExempt;
            adminRole.Active = true;
            await _customerService.UpdateCustomerRoleAsync(adminRole);

            var admin = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            admin.IsTaxExempt = _defaultAdminTaxExempt;
            await _customerService.UpdateCustomerAsync(admin);
        }

        [Test]
        public async Task CanLoadTaxProviders()
        {
            var providers = await _taxPluginManager.LoadAllPluginsAsync();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanLoadTaxProviderBySystemKeyword()
        {
            var provider = await _taxPluginManager.LoadPluginBySystemNameAsync("FixedTaxRateTest");
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanLoadActiveTaxProvider()
        {
            var provider = await _taxPluginManager.LoadPrimaryPluginAsync();
            provider.Should().NotBeNull();
        }

        [Test]
        public async Task CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()
        {
            var customer = new Customer();
            var product = new Product();

            var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
            price.Should().Be(1100);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
            price.Should().Be(1000);
        }

        [Test]
        public async Task CanGetProductPrice()
        {
            var product = new Product();
            var customer = new Customer();

            var (price, _) = await _taxService.GetProductPriceAsync(product, 1000M);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
            price.Should().Be(1000);
        }

        [Test]
        public async Task CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()
        {
            var customer = new Customer();
            var product = new Product();

            //not taxable
            customer.IsTaxExempt = true;

            var (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, true, customer, false);
            price.Should().Be(1000);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, true);
            price.Should().Be(909.0909090909090909090909091M);
            (price, _) = await _taxService.GetProductPriceAsync(product, 0, 1000M, false, customer, false);
            price.Should().Be(1000);
        }
    }
}