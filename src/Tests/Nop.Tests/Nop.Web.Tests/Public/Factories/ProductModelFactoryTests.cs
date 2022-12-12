using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories
{
    [TestFixture]
    public class ProductModelFactoryTests : WebTest
    {
        private IProductModelFactory _productModelFactory;
        private IProductService _productService;
        private IUrlRecordService _urlRecordService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _productModelFactory = GetService<IProductModelFactory>();
            _productService = GetService<IProductService>();
            _urlRecordService = GetService<IUrlRecordService>();
        }

        [Test]
        public async Task CanPrepareProductTemplateViewPath()
        {
            var productTemplateRepository = GetService<IRepository<ProductTemplate>>();
            var productTemplateSimple = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product");
            if (productTemplateSimple == null)
                throw new Exception("Simple product template could not be loaded");
            var productTemplateGrouped = productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped product (with variants)");
            if (productTemplateGrouped == null)
                throw new Exception("Grouped product template could not be loaded");

            var modelSimple = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
            {
                ProductTemplateId = productTemplateSimple.Id
            });

            var modelGrouped = await _productModelFactory.PrepareProductTemplateViewPathAsync(new Product
            {
                ProductTemplateId = productTemplateGrouped.Id
            });

            modelSimple.Should().NotBe(modelGrouped);

            modelSimple.Should().Be(productTemplateSimple.ViewPath);
            modelGrouped.Should().Be(productTemplateGrouped.ViewPath);
        }

        [Test]
        public async Task CanPrepareProductOverviewModels()
        {
            var product = await _productService.GetProductByIdAsync(1);
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();

            PropertiesShouldEqual(product, model);
        }

        [Test]
        public async Task CanPrepareProductDetailsModel()
        {
            var product = await _productService.GetProductByIdAsync(1);
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(new[] { product })).FirstOrDefault();

            PropertiesShouldEqual(product, model);
        }

        [Test]
        public async Task CanPrepareProductReviewsModel()
        {
            var pId = (await _productService.GetProductReviewByIdAsync(1)).ProductId;
            var product = await _productService.GetProductByIdAsync(pId);
            var model = await _productModelFactory.PrepareProductReviewsModelAsync(new ProductReviewsModel(), product);

            model.ProductId.Should().Be(product.Id);
            model.ProductName.Should().Be(product.Name);
            model.ProductSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(product));

            model.Items.Any().Should().BeTrue();
        }

        [Test]
        public async Task CanPrepareCustomerProductReviewsModel()
        {
            var model = await _productModelFactory.PrepareCustomerProductReviewsModelAsync(null);
            var review = model.ProductReviews.FirstOrDefault();

            review.Should().NotBeNull();

            var product = await _productService.GetProductByIdAsync(review.ProductId);

            review.ProductName.Should().Be(product.Name);
            review.ProductSeName.Should().Be(await _urlRecordService.GetSeNameAsync(product));
        }

        [Test]
        public async Task CanPrepareProductEmailAFriendModel()
        {
            var product = await _productService.GetProductByIdAsync(1);
            var model = await _productModelFactory.PrepareProductEmailAFriendModelAsync(new ProductEmailAFriendModel(), product, false);

            model.ProductId.Should().Be(product.Id);
            model.ProductName.Should().Be(product.Name);
            model.ProductSeName.Should().Be(await GetService<IUrlRecordService>().GetSeNameAsync(product));
            model.YourEmailAddress.Should().Be(NopTestsDefaults.AdminEmail);
        }

        [Test]
        public async Task CanPrepareProductSpecificationModel()
        {
            var product = await _productService.GetProductByIdAsync(1);
            var model = await _productModelFactory.PrepareProductSpecificationModelAsync(product);

            var group = model.Groups.FirstOrDefault();

            group.Should().NotBe(null);
        }
    }
}
