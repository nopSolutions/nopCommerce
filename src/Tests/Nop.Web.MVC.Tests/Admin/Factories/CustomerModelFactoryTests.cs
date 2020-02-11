using System.Collections.Generic;
using Moq;
using Nop.Web.Areas.Admin.Factories;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Factories
{
    [TestFixture]
    public class CustomerModelFactoryTests : Nop.Tests.TestsBase
    {
        private CustomerModelFactory _factory;

        #region Dependencies
        protected Mock<Core.Domain.Common.AddressSettings> _addressSettings;
        protected Mock<Core.Domain.Customers.CustomerSettings> _customerSettings;
        protected Mock<Services.Helpers.DateTimeSettings> _dateTimeSettings;
        protected Mock<Core.Domain.Gdpr.GdprSettings> _gdprSettings;
        protected Mock<Framework.Factories.IAclSupportedModelFactory> _aclSupportedModelFactory;
        protected Mock<Services.Common.IAddressAttributeFormatter> _addressAttributeFormatter;
        protected Mock<IAddressAttributeModelFactory> _addressAttributeModelFactory;
        protected Mock<Services.Affiliates.IAffiliateService> _affiliateService;
        protected Mock<Services.Authentication.External.IAuthenticationPluginManager> _authenticationPluginManager;
        protected Mock<Services.Catalog.IBackInStockSubscriptionService> _backInStockSubscriptionService;
        protected Mock<IBaseAdminModelFactory> _baseAdminModelFactory;
        protected Mock<Services.Logging.ICustomerActivityService> _customerActivityService;
        protected Mock<Services.Customers.ICustomerAttributeParser> _customerAttributeParser;
        protected Mock<Services.Customers.ICustomerAttributeService> _customerAttributeService;
        protected Mock<Services.Customers.ICustomerService> _customerService;
        protected Mock<Services.Helpers.IDateTimeHelper> _dateTimeHelper;
        protected Mock<Services.Gdpr.IGdprService> _gdprService;
        protected Mock<Services.Common.IGenericAttributeService> _genericAttributeService;
        protected Mock<Services.Directory.IGeoLookupService> _geoLookupService;
        protected Mock<Services.Localization.ILocalizationService> _localizationService;
        protected Mock<Services.Messages.INewsLetterSubscriptionService> _newsLetterSubscriptionService;
        protected Mock<Services.Orders.IOrderService> _orderService;
        protected Mock<Services.Media.IPictureService> _pictureService;
        protected Mock<Services.Catalog.IPriceCalculationService> _priceCalculationService;
        protected Mock<Services.Catalog.IPriceFormatter> _priceFormatter;
        protected Mock<Services.Catalog.IProductAttributeFormatter> _productAttributeFormatter;
        protected Mock<Services.Orders.IRewardPointService> _rewardPointService;
        protected Mock<Core.IStoreContext> _storeContext;
        protected Mock<Services.Stores.IStoreService> _storeService;
        protected Mock<Services.Tax.ITaxService> _taxService;
        protected Mock<Core.Domain.Media.MediaSettings> _mediaSettings;
        protected Mock<Core.Domain.Customers.RewardPointsSettings> _rewardPointsSettings;
        protected Mock<Core.Domain.Tax.TaxSettings> _taxSettings;
        #endregion

        [SetUp]
        public new void Setup()
        {

            _addressSettings = new Mock<Core.Domain.Common.AddressSettings>();
            _customerSettings = new Mock<Core.Domain.Customers.CustomerSettings>();
            _dateTimeSettings = new Mock<Services.Helpers.DateTimeSettings>();
            _gdprSettings = new Mock<Core.Domain.Gdpr.GdprSettings>();
            _aclSupportedModelFactory = new Mock<Framework.Factories.IAclSupportedModelFactory>();
            _addressAttributeFormatter = new Mock<Services.Common.IAddressAttributeFormatter>();
            _addressAttributeModelFactory = new Mock<IAddressAttributeModelFactory>();
            _affiliateService = new Mock<Services.Affiliates.IAffiliateService>();
            _authenticationPluginManager = new Mock<Services.Authentication.External.IAuthenticationPluginManager>();
            _backInStockSubscriptionService = new Mock<Services.Catalog.IBackInStockSubscriptionService>();
            _baseAdminModelFactory = new Mock<IBaseAdminModelFactory>();
            _customerActivityService = new Mock<Services.Logging.ICustomerActivityService>();
            _customerAttributeParser = new Mock<Services.Customers.ICustomerAttributeParser>();
            _customerAttributeService = new Mock<Services.Customers.ICustomerAttributeService>();
            _customerService = new Mock<Services.Customers.ICustomerService>();
            _dateTimeHelper = new Mock<Services.Helpers.IDateTimeHelper>();
            _gdprService = new Mock<Services.Gdpr.IGdprService>();
            _genericAttributeService = new Mock<Services.Common.IGenericAttributeService>();
            _geoLookupService = new Mock<Services.Directory.IGeoLookupService>();
            _localizationService = new Mock<Services.Localization.ILocalizationService>();
            _newsLetterSubscriptionService = new Mock<Services.Messages.INewsLetterSubscriptionService>();
            _orderService = new Mock<Services.Orders.IOrderService>();
            _pictureService = new Mock<Services.Media.IPictureService>();
            _priceCalculationService = new Mock<Services.Catalog.IPriceCalculationService>();
            _priceFormatter = new Mock<Services.Catalog.IPriceFormatter>();
            _productAttributeFormatter = new Mock<Services.Catalog.IProductAttributeFormatter>();
            _rewardPointService = new Mock<Services.Orders.IRewardPointService>();
            _storeContext = new Mock<Core.IStoreContext>();
            _storeService = new Mock<Services.Stores.IStoreService>();
            _taxService = new Mock<Services.Tax.ITaxService>();
            _mediaSettings = new Mock<Core.Domain.Media.MediaSettings>();
            _rewardPointsSettings = new Mock<Core.Domain.Customers.RewardPointsSettings>();
            _taxSettings = new Mock<Core.Domain.Tax.TaxSettings>();

            _factory = new CustomerModelFactory(
                _addressSettings.Object,
                _customerSettings.Object,
                _dateTimeSettings.Object,
                _gdprSettings.Object,
                _aclSupportedModelFactory.Object,
                _addressAttributeFormatter.Object,
                _addressAttributeModelFactory.Object,
                _affiliateService.Object,
                _authenticationPluginManager.Object,
                _backInStockSubscriptionService.Object,
                _baseAdminModelFactory.Object,
                _customerActivityService.Object,
                _customerAttributeParser.Object,
                _customerAttributeService.Object,
                _customerService.Object,
                _dateTimeHelper.Object,
                _gdprService.Object,
                _genericAttributeService.Object,
                _geoLookupService.Object,
                _localizationService.Object,
                _newsLetterSubscriptionService.Object,
                _orderService.Object,
                _pictureService.Object,
                _priceCalculationService.Object,
                _priceFormatter.Object,
                _productAttributeFormatter.Object,
                _rewardPointService.Object,
                _storeContext.Object,
                _storeService.Object,
                _taxService.Object,
                _mediaSettings.Object,
                _rewardPointsSettings.Object,
                _taxSettings.Object
            );
        }

        [Test]
        public void should_hide_registered_in_store_label_when_it_has_only_one_store()
        {
            _customerAttributeService.Setup(x => x.GetAllCustomerAttributes())
                .Returns(new List<Core.Domain.Customers.CustomerAttribute>());

            _storeService.Setup(x => x.GetAllStores(It.IsAny<bool>()))
                .Returns(new List<Core.Domain.Stores.Store>() { 
                    new Core.Domain.Stores.Store() { Id = 1, Name = "My Store" } 
                });

            var customer = new Core.Domain.Customers.Customer { Id = 1, RegisteredInStoreId = 1 };
            var customerModel = new Areas.Admin.Models.Customers.CustomerModel { Id = 1 };

            var model = _factory.PrepareCustomerModel(customerModel, customer);

            Assert.IsFalse(model.DisplayRegisteredInStore);
        }

        [Test]
        public void should_display_registered_in_store_label_when_it_has_multiple_stores()
        {
            _customerAttributeService.Setup(x => x.GetAllCustomerAttributes())
                .Returns(new List<Core.Domain.Customers.CustomerAttribute>());

            _storeService.Setup(x => x.GetAllStores(It.IsAny<bool>()))
                .Returns(new List<Core.Domain.Stores.Store>() { 
                    new Core.Domain.Stores.Store() { Id = 1, Name = "First Store" },
                    new Core.Domain.Stores.Store() { Id = 2, Name = "Second Store" }
                });

            var customer = new Core.Domain.Customers.Customer { Id = 1, RegisteredInStoreId = 1 };
            var customerModel = new Areas.Admin.Models.Customers.CustomerModel { Id = 1 };

            var model = _factory.PrepareCustomerModel(customerModel, customer);

            Assert.IsTrue(model.DisplayRegisteredInStore);
        }
    }
}
