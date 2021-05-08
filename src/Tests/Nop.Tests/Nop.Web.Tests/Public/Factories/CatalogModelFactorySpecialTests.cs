using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories
{
    [TestFixture]
    public class CatalogModelFactorySpecialTests: WebTest
    {
        private ISettingService _settingsService;
        private CatalogSettings _catalogSettings;
        private Category _category;
        private VendorSettings _vendorSettings;
        private ICatalogModelFactory _catalogModelFactory;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _settingsService = GetService<ISettingService>();

            var categoryService = GetService<ICategoryService>();
            _category = await categoryService.GetCategoryByIdAsync(1);

            _vendorSettings = GetService<VendorSettings>();
            _vendorSettings.VendorsBlockItemsToDisplay = 1;
            _vendorSettings.AllowSearchByVendor = true;

            await _settingsService.SaveSettingAsync(_vendorSettings);
            
            _catalogSettings = GetService<CatalogSettings>();

            _catalogSettings.AllowProductViewModeChanging = false;
            _catalogSettings.CategoryBreadcrumbEnabled = false;
            _catalogSettings.ShowProductsFromSubcategories = true;
            _catalogSettings.ShowCategoryProductNumber = true;
            _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = true;
            _catalogSettings.NumberOfProductTags = 20;

            await _settingsService.SaveSettingAsync(_catalogSettings);

            _catalogModelFactory = GetService<ICatalogModelFactory>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _vendorSettings.VendorsBlockItemsToDisplay = 0;
            _vendorSettings.AllowSearchByVendor = false;
            await _settingsService.SaveSettingAsync(_vendorSettings);

            _catalogSettings.AllowProductViewModeChanging = true;
            _catalogSettings.CategoryBreadcrumbEnabled = true;
            _catalogSettings.ShowProductsFromSubcategories = false;
            _catalogSettings.ShowCategoryProductNumber = false;
            _catalogSettings.ShowCategoryProductNumberIncludingSubcategories = false;
            _catalogSettings.NumberOfProductTags = 15;
            await _settingsService.SaveSettingAsync(_catalogSettings);
        }

        [Test]
        public async Task CanPrepareVendorNavigationModel()
        {
            var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
            model.TotalVendors.Should().Be(2);
            model.Vendors.Any().Should().BeTrue();
            model.Vendors.Count.Should().Be(1);
            model.Vendors[0].Name.Should().Be("Vendor 1");
        }

        [Test]
        public async Task PrepareSearchModelShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogProductsCommand());
            
            model.AvailableVendors.Any().Should().BeTrue();
            model.AvailableVendors.Count.Should().Be(3);
        }
        
        [Test]
        public async Task PrepareCategoryModelShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogProductsCommand());
           
            model.CategoryBreadcrumb.Any().Should().BeFalse();
            model.SubCategories.Count.Should().Be(3);
            model.CatalogProductsModel.Products.Count.Should().Be(6);
        }
        
        [Test]
        public async Task CanPreparePopularProductTagsModel()
        {
            var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync(_catalogSettings.NumberOfProductTags);

            model.Tags.Count.Should().Be(16);
            model.TotalTags.Should().Be(16);
        }

        [Test]
        public async Task PrepareViewModesShouldDependOnSettings()
        {
            var model = new CatalogProductsModel();
            await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogProductsCommand
            {
                ViewMode = "list"
            });

            model.AllowProductViewModeChanging.Should().BeFalse();
            model.AvailableViewModes.Count.Should().Be(0);
            model.ViewMode.Should().Be("list");
        }

        [Test]
        public async Task PrepareCategorySimpleModelsShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareCategorySimpleModelsAsync();

            var numberOfProducts = model
                .FirstOrDefault(p => p.Id == _category.Id)?.NumberOfProducts;

            numberOfProducts.Should().Be(12);
        }
    }
}