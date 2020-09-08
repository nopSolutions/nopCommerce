using System.Linq;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Tax
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

        [SetUp]
        public void SetUp()
        {
            _taxService = GetService<ITaxService>();
            _taxPluginManager = GetService<ITaxPluginManager>();
            _taxSettings = GetService<TaxSettings>();
            _settingService = GetService<ISettingService>();
            _customerService = GetService<ICustomerService>();

            _defaultEuVatAssumeValid = _taxSettings.EuVatAssumeValid;
            _taxSettings.EuVatAssumeValid = false;
            _settingService.SaveSetting(_taxSettings);

            var adminRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.AdministratorsRoleName);
            _defaultAdminRoleTaxExempt = adminRole.TaxExempt;
            var admin = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            _defaultAdminTaxExempt = admin.IsTaxExempt;
        }

        [TearDown]
        public void TearDown()
        {
            _taxSettings.EuVatAssumeValid = _defaultEuVatAssumeValid;
            _settingService.SaveSetting(_taxSettings);

            var adminRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.AdministratorsRoleName);
            adminRole.TaxExempt = _defaultAdminRoleTaxExempt;
            adminRole.Active = true;
            _customerService.UpdateCustomerRole(adminRole);

            var admin = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            admin.IsTaxExempt = _defaultAdminTaxExempt;
            _customerService.UpdateCustomer(admin);
        }

        [Test]
        public void CanLoadTaxProviders()
        {
            var providers = _taxPluginManager.LoadAllPlugins();
            providers.Should().NotBeNull();
            providers.Any().Should().BeTrue();
        }

        [Test]
        public void CanLoadTaxProviderBySystemKeyword()
        {
            var provider = _taxPluginManager.LoadPluginBySystemName("FixedTaxRateTest");
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanLoadActiveTaxProvider()
        {
            var provider = _taxPluginManager.LoadPrimaryPlugin();
            provider.Should().NotBeNull();
        }

        [Test]
        public void CanCheckTaxExemptProduct()
        {
            var product = new Product
            {
                IsTaxExempt = true
            };
            _taxService.IsTaxExempt(product, null).Should().BeTrue();
            product.IsTaxExempt = false;
            _taxService.IsTaxExempt(product, null).Should().BeFalse();
        }

        [Test]
        public void CanCheckTaxExemptCustomer()
        {
            var customer = new Customer
            {
                IsTaxExempt = true
            };
            _taxService.IsTaxExempt(null, customer).Should().BeTrue();
            customer.IsTaxExempt = false;
            _taxService.IsTaxExempt(null, customer).Should().BeFalse();
        }

        [Test]
        public void CanCheckTaxExemptCustomerInTaxExemptCustomerRole()
        {
            var customer = _customerService.GetCustomerByEmail(NopTestsDefaults.AdminEmail);
            customer.IsTaxExempt = false;
            _customerService.UpdateCustomer(customer);

            GetService<ITaxService>().IsTaxExempt(null, customer).Should().BeFalse();

            var customerRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.AdministratorsRoleName);
            customerRole.Should().NotBeNull();

            customerRole.TaxExempt = true;
            _customerService.UpdateCustomerRole(customerRole);
            GetService<ITaxService>().IsTaxExempt(null, customer).Should().BeTrue();

            customerRole.TaxExempt = false;
            _customerService.UpdateCustomerRole(customerRole);
            GetService<ITaxService>().IsTaxExempt(null, customer).Should().BeFalse();

            //if role is not active, we should ignore 'TaxExempt' property
            customerRole.TaxExempt = true;
            customerRole.Active = false;
            _customerService.UpdateCustomerRole(customerRole);
            GetService<ITaxService>().IsTaxExempt(null, customer).Should().BeFalse();
        }

        [Test]
        public void CanGetProductPricePriceIncludesTaxIncludingTaxTaxable()
        {
            var customer = new Customer();
            var product = new Product();

            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out _).Should().Be(1000);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out _).Should().Be(1100);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out _).Should().Be(1000);
        }

        [Test]
        public void CanGetProductPricePriceIncludesTaxIncludingTaxNonTaxable()
        {
            var customer = new Customer();
            var product = new Product();

            //not taxable
            customer.IsTaxExempt = true;

            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out _).Should().Be(1000);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out _).Should().Be(1000);
        }

        [Test]
        public void CanDoVatCheck()
        {
            var vatNumberStatus1 = _taxService.DoVatCheck("GB", "523 2392 69",
                out _, out _, out var exception);

            if (exception != null)
            {
                TestContext.WriteLine($"Can't run the \"Can_do_VAT_check\":\r\n{exception.Message}");
                return;
            }

            vatNumberStatus1.Should().Be(VatNumberStatus.Valid);

            var vatNumberStatus2 = _taxService.DoVatCheck("GB", "000 0000 00",
                out _, out _, out exception);

            if (exception != null)
            {
                TestContext.WriteLine($"Can't run the \"Can_do_VAT_check\":\r\n{exception.Message}");
                return;
            }

            vatNumberStatus2.Should().Be(VatNumberStatus.Invalid);
        }

        [Test]
        public void ShouldAssumeValidVatNumberIfEuVatAssumeValidSettingIsTrue()
        {
            _taxSettings.EuVatAssumeValid = true;
            _settingService.SaveSetting(_taxSettings);

            var vatNumberStatus = GetService<ITaxService>().GetVatNumberStatus("GB", "000 0000 00", out _, out _);
            vatNumberStatus.Should().Be(VatNumberStatus.Valid);
        }
    }
}
