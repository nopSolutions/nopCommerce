using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Tax;
using Nop.Services.Tests.FakeServices.Providers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests : ServiceTest
    {
        private Mock<IAddressService> _addressService;
        private IWorkContext _workContext;
        private Mock<IStoreContext> _storeContext;
        private TaxSettings _taxSettings;
        private Mock<IEventPublisher> _eventPublisher;
        private ITaxPluginManager _taxPluginManager;
        private ITaxService _taxService;
        private Mock<IGeoLookupService> _geoLookupService;
        private Mock<ICountryService> _countryService;
        private CustomerService _customerService;
        private Mock<IStateProvinceService> _stateProvinceService;
        private Mock<ILogger> _logger;
        private Mock<IWebHelper> _webHelper;
        private CustomerSettings _customerSettings;
        private ShippingSettings _shippingSettings;
        private AddressSettings _addressSettings;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<IRepository<CustomerCustomerRoleMapping>> _customerCustomerRoleMappingRepo;
        private Mock<IRepository<CustomerRole>> _customerRoleRepo;

        [SetUp]
        public new void SetUp()
        {
            _taxSettings = new TaxSettings
            {
                DefaultTaxAddressId = 10
            };

            _workContext = null;
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(new Store { Id = 1 });

            _addressService = new Mock<IAddressService>();
            //default tax address
            _addressService.Setup(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Returns(new Address { Id = _taxSettings.DefaultTaxAddressId });

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _geoLookupService = new Mock<IGeoLookupService>();
            _countryService = new Mock<ICountryService>();

            _customerRoleRepo = new Mock<IRepository<CustomerRole>>();

            _customerRoleRepo.Setup(r => r.Table).Returns(new List<CustomerRole>
            {
                new CustomerRole
                {
                    Id = 1,
                    TaxExempt = true,
                    Active = true
                }
            }.AsQueryable());

            _customerCustomerRoleMappingRepo = new Mock<IRepository<CustomerCustomerRoleMapping>>();
            var mappings = new List<CustomerCustomerRoleMapping>();

            _customerCustomerRoleMappingRepo.Setup(r => r.Table).Returns(mappings.AsQueryable());
            _customerCustomerRoleMappingRepo.Setup(r => r.Insert(It.IsAny<CustomerCustomerRoleMapping>())).Callback(
                (CustomerCustomerRoleMapping ccrm) => { mappings.Add(ccrm); });

            _stateProvinceService = new Mock<IStateProvinceService>();
            _logger = new Mock<ILogger>();
            _webHelper = new Mock<IWebHelper>();
            _genericAttributeService = new Mock<IGenericAttributeService>();

            _customerSettings = new CustomerSettings();
            _shippingSettings = new ShippingSettings();
            _addressSettings = new AddressSettings();

            _customerService = new CustomerService(new CachingSettings(),
                new CustomerSettings(),
                new FakeCacheKeyService(),
                _eventPublisher.Object,
                _genericAttributeService.Object,
                null,
                null,
                null,
                _customerCustomerRoleMappingRepo.Object,
                null,
                _customerRoleRepo.Object,
                null,
                null,
                new TestCacheManager(),
                _storeContext.Object,
                null);

            var pluginService = new FakePluginService();
            _taxPluginManager = new TaxPluginManager(_customerService, pluginService, _taxSettings);

            _taxService = new TaxService(_addressSettings,
                _customerSettings,
                _addressService.Object,
                _countryService.Object,
                _customerService,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _geoLookupService.Object,
                _logger.Object,
                _stateProvinceService.Object,
                _storeContext.Object,
                _taxPluginManager,
                _webHelper.Object,
                _workContext,
                _shippingSettings,
                _taxSettings);
        }

        [Test]
        public void Can_load_taxProviders()
        {
            RunWithTestServiceProvider(() =>
            {
                var providers = _taxPluginManager.LoadAllPlugins();
                providers.Should().NotBeNull();
                providers.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_load_taxProvider_by_systemKeyword()
        {
            RunWithTestServiceProvider(() =>
            {
                var provider = _taxPluginManager.LoadPluginBySystemName("FixedTaxRateTest");
                provider.Should().NotBeNull();
            });
        }

        [Test]
        public void Can_load_active_taxProvider()
        {
            var serviceProvider = new FakeServiceProvider(_genericAttributeService.Object, _taxService, _taxSettings);
            var nopEngine = new FakeNopEngine(serviceProvider);
            EngineContext.Replace(nopEngine);

            var provider = _taxPluginManager.LoadPrimaryPlugin();
            provider.Should().NotBeNull();

            EngineContext.Replace(null);
        }

        [Test]
        public void Can_check_taxExempt_product()
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
        public void Can_check_taxExempt_customer()
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
        public void Can_check_taxExempt_customer_in_taxExemptCustomerRole()
        {
            var customer = new Customer
            {
                Id = 1,
                IsTaxExempt = false
            };
            _taxService.IsTaxExempt(null, customer).Should().BeFalse();

            var customerRole = _customerRoleRepo.Object.Table.FirstOrDefault(cr => cr.Id == 1);

            customerRole.Should().NotBeNull();

            _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });

            _taxService.IsTaxExempt(null, customer).Should().BeTrue();

            customerRole.TaxExempt = false;
            _taxService.IsTaxExempt(null, customer).Should().BeFalse();

            //if role is not active, we should ignore 'TaxExempt' property
            customerRole.Active = false;
            _taxService.IsTaxExempt(null, customer).Should().BeFalse();
        }

        [Test]
        public void Can_get_productPrice_priceIncludesTax_includingTax_taxable()
        {
            var customer = new Customer();
            var product = new Product();

            var serviceProvider = new FakeServiceProvider(_genericAttributeService.Object, _taxService, _taxSettings);
            var nopEngine = new FakeNopEngine(serviceProvider);
            EngineContext.Replace(nopEngine);
            
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out _).Should().Be(1000);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out _).Should().Be(1100);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out _).Should().Be(1000);
            
            EngineContext.Replace(null);
        }

        [Test]
        public void Can_get_productPrice_priceIncludesTax_includingTax_non_taxable()
        {
            var customer = new Customer();
            var product = new Product();

            //not taxable
            customer.IsTaxExempt = true;

            var serviceProvider = new FakeServiceProvider(_genericAttributeService.Object, _taxService, _taxSettings);
            var nopEngine = new FakeNopEngine(serviceProvider);
            EngineContext.Replace(nopEngine);

            _taxService.GetProductPrice(product, 0, 1000M, true, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, true, customer, false, out _).Should().Be(1000);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, true, out _).Should()
                .Be(909.0909090909090909090909091M);
            _taxService.GetProductPrice(product, 0, 1000M, false, customer, false, out _).Should().Be(1000);

            EngineContext.Replace(null);
        }

        [Test]
        public void Can_do_VAT_check()
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
        public void Should_assume_valid_VAT_number_if_EuVatAssumeValid_setting_is_true()
        {
            _taxSettings.EuVatAssumeValid = true;

            var vatNumberStatus = _taxService.GetVatNumberStatus("GB", "000 0000 00", out _, out _);
            vatNumberStatus.Should().Be(VatNumberStatus.Valid);
        }
    }
}
