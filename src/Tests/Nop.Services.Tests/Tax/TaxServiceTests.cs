using System;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests : ServiceTest
    {
        IAddressService _addressService;
        IWorkContext _workContext;
        TaxSettings _taxSettings;
        IEventPublisher _eventPublisher;
        ITaxService _taxService;

        [SetUp]
        public new void SetUp()
        {
            _taxSettings = new TaxSettings();
            _taxSettings.DefaultTaxAddressId = 10;

            _workContext = null;

            _addressService = MockRepository.GenerateMock<IAddressService>();
            //default tax address
            _addressService.Expect(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Return(new Address() { Id = _taxSettings.DefaultTaxAddressId });

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _taxService = new TaxService(_addressService, _workContext, _taxSettings, pluginFinder, _eventPublisher);
        }

        [Test]
        public void Can_load_taxProviders()
        {
            var providers = _taxService.LoadAllTaxProviders();
            providers.ShouldNotBeNull();
            (providers.Count > 0).ShouldBeTrue();
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
        public void Can_check_taxExempt_productVariant()
        {
            var productVariant = new ProductVariant();
            productVariant.IsTaxExempt = true;
            _taxService.IsTaxExempt(productVariant, null).ShouldEqual(true);
            productVariant.IsTaxExempt = false;
            _taxService.IsTaxExempt(productVariant, null).ShouldEqual(false);
        }

        [Test]
        public void Can_check_taxExempt_customer()
        {
            var customer = new Customer();
            customer.IsTaxExempt = true;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(true);
            customer.IsTaxExempt = false;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);
        }

        [Test]
        public void Can_check_taxExempt_customer_in_taxExemptCustomerRole()
        {
            var customer = new Customer();
            customer.IsTaxExempt = false;
            _taxService.IsTaxExempt(null, customer).ShouldEqual(false);

            var customerRole = new CustomerRole()
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
            //10 is a fixed tax rate returned from FixedRateTestTaxProvider. Perhaps, it should be configured in some other way 
            return 10;
        }

        [Test]
        public void Can_get_tax_rate_for_productVariant()
        {
            _taxSettings.TaxBasedOn = TaxBasedOn.BillingAddress;

            var customer = new Customer();
            customer.BillingAddress = new Address();
            var productVariant = new ProductVariant();

            _taxService.GetTaxRate(productVariant, customer).ShouldEqual(GetFixedTestTaxRate());
            productVariant.IsTaxExempt = true;
            _taxService.GetTaxRate(productVariant, customer).ShouldEqual(0);
        }

        [Test]
        public void Can_get_productPrice_priceIncludesTax_includingTax()
        {
            var customer = new Customer();
            var productVariant = new ProductVariant();

            decimal taxRate;
            _taxService.GetProductPrice(productVariant, 0, 1000M, true, customer, true, out taxRate).ShouldEqual(1000);
            _taxService.GetProductPrice(productVariant, 0, 1000M, true, customer, false, out taxRate).ShouldEqual(1100);
            _taxService.GetProductPrice(productVariant, 0, 1000M, false, customer, true, out taxRate).ShouldEqual(909.0909090909090909090909091M);
            _taxService.GetProductPrice(productVariant, 0, 1000M, false, customer, false, out taxRate).ShouldEqual(1000);
        }

        [Test]
        public void Can_do_VAT_check()
        {
            //remove? this method requires Internet access

            string name, address;
            Exception exception;

            VatNumberStatus vatNumberStatus1 = _taxService.DoVatCheck("GB", "523 2392 69",
                out name, out address, out exception);
            vatNumberStatus1.ShouldEqual(VatNumberStatus.Valid);
            exception.ShouldBeNull();

            VatNumberStatus vatNumberStatus2 = _taxService.DoVatCheck("GB", "000 0000 00",
                out name, out address, out exception);
            vatNumberStatus2.ShouldEqual(VatNumberStatus.Invalid);
            exception.ShouldBeNull();
        }
    }
}
