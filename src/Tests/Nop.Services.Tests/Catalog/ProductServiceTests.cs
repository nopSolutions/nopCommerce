using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Messages;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductServiceTests : ServiceTest
    {
        IRepository<Product> _productRepository;
        IRepository<ProductVariant> _productVariantRepository;
        IRepository<RelatedProduct> _relatedProductRepository;
        IRepository<CrossSellProduct> _crossSellProductRepository;
        IRepository<TierPrice> _tierPriceRepository;
        IRepository<ProductPicture> _productPictureRepository;
        IProductAttributeService _productAttributeService;
        IProductAttributeParser _productAttributeParser;
        IWorkflowMessageService _workflowMessageService;
        LocalizationSettings _localizationSettings;
        IEventPublisher _eventPublisher;

        IProductService _productService;

        [SetUp]
        public new void SetUp()
        {
            _productRepository = MockRepository.GenerateMock<IRepository<Product>>();
            _productVariantRepository = MockRepository.GenerateMock<IRepository<ProductVariant>>();
            _relatedProductRepository = MockRepository.GenerateMock<IRepository<RelatedProduct>>();
            _crossSellProductRepository = MockRepository.GenerateMock<IRepository<CrossSellProduct>>();
            _tierPriceRepository = MockRepository.GenerateMock<IRepository<TierPrice>>();
            _productPictureRepository = MockRepository.GenerateMock<IRepository<ProductPicture>>();
            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            var cacheManager = new NopNullCache();

            _productAttributeService = MockRepository.GenerateMock<IProductAttributeService>();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();
            _workflowMessageService = MockRepository.GenerateMock<IWorkflowMessageService>();
            _localizationSettings = new LocalizationSettings();

            _productService = new ProductService(cacheManager,
                _productRepository, _productVariantRepository,
                _relatedProductRepository, _crossSellProductRepository,
                _tierPriceRepository,_productPictureRepository,
                _productAttributeService, 
                _productAttributeParser, 
                _workflowMessageService, 
                _localizationSettings, 
                _eventPublisher);
        }

        //TODO write unit tests for SearchProducts method
    }
}
