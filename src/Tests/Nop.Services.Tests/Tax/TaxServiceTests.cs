using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Services.Security;
using Nop.Tests;
using NUnit.Framework;
using Nop.Core;
using Nop.Services.Discounts;
using Nop.Core.Domain.Tax;
using Rhino.Mocks;
using Nop.Services.Common;
using Nop.Services.Tax;
using Nop.Core.Infrastructure;

namespace Nop.Services.Tests.Tax
{
    [TestFixture]
    public class TaxServiceTests
    {
        IAddressService _addressService;
        IWorkContext _workContext;
        TaxSettings _taxSettings;
        ITaxService _taxService;

        [SetUp]
        public void SetUp()
        {
            _taxSettings = new TaxSettings();
            _taxService = new TaxService(_addressService, _workContext, _taxSettings, new TypeFinder());
        }

        [Test]
        public void Can_load_taxProviders()
        {
            var providers = _taxService.LoadAllTaxProviders();
            providers.ShouldNotBeNull();
            (providers.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_load_taxProviderBySystemKeyword()
        {
            var rule = _taxService.LoadTaxProviderBySystemName("FixedTaxRateTest");
            rule.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_activeTaxProvider()
        {
            var provider = _taxService.LoadActiveTaxProvider();
            provider.ShouldNotBeNull();
        }
    }
}
