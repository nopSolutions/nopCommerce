using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Catalog
{
    [TestFixture]
    public class ProductServiceTests
    {
        IRepository<Product> _productRepository;
        IRepository<ProductVariant> _productVariantRepository;
        IRepository<RelatedProduct> _relatedProductRepository;
        IRepository<CrossSellProduct> _crossSellProductRepository;
        IRepository<TierPrice> _tierPriceRepository;
        IRepository<ProductPicture> _productPictureRepository;
        IProductAttributeService _productAttributeService;
        IProductAttributeParser _productAttributeParser;
        ICacheManager _cacheManager;

        IProductService _productService;
        
        [SetUp]
        public void SetUp()
        {
            _productRepository = MockRepository.GenerateMock<IRepository<Product>>();
            _productVariantRepository = MockRepository.GenerateMock<IRepository<ProductVariant>>();
            _relatedProductRepository = MockRepository.GenerateMock<IRepository<RelatedProduct>>();
            _crossSellProductRepository = MockRepository.GenerateMock<IRepository<CrossSellProduct>>();
            _tierPriceRepository = MockRepository.GenerateMock<IRepository<TierPrice>>();
            _productPictureRepository = MockRepository.GenerateMock<IRepository<ProductPicture>>();

            var cacheManager = new NopNullCache();

            _productAttributeService = MockRepository.GenerateMock<IProductAttributeService>();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();

            _productService = new ProductService(_cacheManager,
            _productRepository, _productVariantRepository,
            _relatedProductRepository, _crossSellProductRepository,
            _tierPriceRepository,_productPictureRepository,
            _productAttributeService, _productAttributeParser);
        }

        //TODO write unit tests for SearchProducts method
    }
}
