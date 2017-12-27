using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests : ServiceTest
    {
        private IAddressService _addressService;
        private IWorkContext _workContext;
        private IStoreContext _storeContext;
        private TaxSettings _taxSettings;
        private IEventPublisher _eventPublisher;
        private ITaxService _taxService;
        private IGeoLookupService _geoLookupService;
        private ICountryService _countryService;
        private IStateProvinceService _stateProvinceService;
        private ILogger _logger;
        private IWebHelper _webHelper;
        private CustomerSettings _customerSettings;
        private ShippingSettings _shippingSettings;
        private AddressSettings _addressSettings;

        [SetUp]
        public new void SetUp()
        {
            _taxSettings = new TaxSettings
            {
                DefaultTaxAddressId = 10
            };

            _workContext = null;
            _storeContext = null;

            _addressService = MockRepository.GenerateMock<IAddressService>();
            //default tax address
            _addressService.Expect(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Return(new Address { Id = _taxSettings.DefaultTaxAddressId });

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            var pluginFinder = new PluginFinder();

            _geoLookupService = MockRepository.GenerateMock<IGeoLookupService>();
            _countryService = MockRepository.GenerateMock<ICountryService>();
            _stateProvinceService = MockRepository.GenerateMock<IStateProvinceService>();
            _logger = MockRepository.GenerateMock<ILogger>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();

            _customerSettings = new CustomerSettings();
            _shippingSettings = new ShippingSettings();
            _addressSettings = new AddressSettings();

            _taxService = new TaxService(_addressService, _workContext, _storeContext, _taxSettings,
                pluginFinder, _geoLookupService, _countryService, _stateProvinceService, _logger, _webHelper,
                _customerSettings, _shippingSettings, _addressSettings);
        }

        [Test]
        public void Can_load_taxProviders()
        {
            var providers = _taxService.LoadAllTaxProviders();
            providers.ShouldNotBeNull();
            (providers.Any()).ShouldBeTrue();
        }

        [Test]
        public void Can_load_taxProvider_by_systemKeyword()
        {
            var provider = _taxService.LoadTaxProviderBySystemName("FixedTaxRateTest");
            provider.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_taxProvider()
        {
            var provider = _taxService.LoadActiveTaxProvider();
            provider.ShouldNotBeNull();
        }

        [Test]
        public void Can_check_taxExempt_product()
        {
            var product = new Product
            {
                IsTaxExempt = true
            };
            _taxService.IsTaxExempt(product, null).ShouldEqual(true);
            product.IsTaxExempt = false;
            _taxService.IsTaxExempt(product, null).ShouldEqual(false);
        }

        [Test]
        public void Can_check_taxExempt_customer()
        {
            var customer = new Customer
            {
                IsTaxExempt = true
            };
            _taxService.IsTaxExempt(null, customer).ShouldEqual(true);
            customer.IsTaxExempt = false;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);
        }

        [Test]
        public void Can_check_taxExempt_customer_in_taxExemptCustomerRole()
        {
            var customer = new Customer
            {
                IsTaxExempt = false
            };
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);

            var customerRole = new CustomerRole
            {
                TaxExempt = true,
                Active = true
            };
            customer.CustomerRoles.Add(customerRole);
            _taxService.IsTaxExempt(null, customer).ShouldEqual(true);
            customerRole.TaxExempt = false;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);

            //if role is not active, weshould ignore 'TaxExempt' property
            customerRole.Active = false;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);
        }

        protected decimal GetFixedTestTaxRate()
        {
            //10 is a fixed tax rate returned from FixedRateTestTaxProvider. Perhaps, it should be configured some other way 
            return 10;
        }

        [Test]
        public void Can_get_productPrice_priceIncludesTax_includingTax_taxable()
        {
            var customer = new Customer();
            var product = new Product();

            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out decimal taxRate).ShouldEqual(1000);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out taxRate).ShouldEqual(1100);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out taxRate).ShouldEqual(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out taxRate).ShouldEqual(1000);
        }

        [Test]
        public void Can_get_productPrice_priceIncludesTax_includingTax_non_taxable()
        {
            var customer = new Customer();
            var product = new Product();

            //not taxable
            customer.IsTaxExempt = true;

            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out decimal taxRate).ShouldEqual(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out taxRate).ShouldEqual(1000);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out taxRate).ShouldEqual(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out taxRate).ShouldEqual(1000);
        }

        [Test]
        public void Can_do_VAT_check()
        {
            //remove? this method requires Internet access
            
            var vatNumberStatus1 = _taxService.DoVatCheck("GB", "523 2392 69",
                out string name, out string address, out Exception exception);
            vatNumberStatus1.ShouldEqual(VatNumberStatus.Valid);
            exception.ShouldBeNull();

            var vatNumberStatus2 = _taxService.DoVatCheck("GB", "000 0000 00",
                out name, out address, out exception);
            vatNumberStatus2.ShouldEqual(VatNumberStatus.Invalid);
            exception.ShouldBeNull();
        }

        [Test]
        public void Should_assume_valid_VAT_number_if_EuVatAssumeValid_setting_is_true()
        {
            _taxSettings.EuVatAssumeValid = true;

            var vatNumberStatus = _taxService.GetVatNumberStatus("GB", "000 0000 00", out string _, out string _);
            vatNumberStatus.ShouldEqual(VatNumberStatus.Valid);
        }
    }
}
