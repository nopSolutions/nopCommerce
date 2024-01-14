using AO.Services.Models;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class ManufacturerCategoryService : IManufacturerCategoryService
    {

        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ManufacturerCategory> _manufacturerCategoryRepository;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductService _productService;

        public ManufacturerCategoryService(
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository,
            IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityService,
            IProductService productService,
            IRepository<ManufacturerCategory> manufacturerCategoryRepository,
            IRepository<Product> productRepository)
        {
            _productManufacturerRepository = productManufacturerRepository;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _urlRecordService = urlRecordService;
            _localizedEntityService = localizedEntityService;
            _productService = productService;
            _manufacturerCategoryRepository = manufacturerCategoryRepository;
            _productRepository = productRepository;
        }

        public async Task<IList<ManufacturerCategory>> GetManufacturerCategoriesAsync(IList<Category> categories, Manufacturer manufacturer, int languageId)
        {
            var manufacturerCategories = new List<ManufacturerCategory>();

            foreach (var category in categories)
            {
                var hasProducts = await (from pm in _productManufacturerRepository.Table
                                         join pc in _productCategoryRepository.Table on pm.ProductId equals pc.ProductId
                                         join p in _productRepository.Table on pm.ProductId equals p.Id
                                         where pm.ManufacturerId == manufacturer.Id
                                            && pc.CategoryId == category.Id
                                            && p.Published
                                            && !p.Deleted
                                         select pm.ProductId).AnyAsync();


                if (hasProducts == false)
                {
                    continue;
                }

                var categoryName = await _localizedEntityService.GetLocalizedValueAsync(languageId, category.Id, "Category", "Name");
                if(string.IsNullOrWhiteSpace(categoryName))
                {
                    categoryName = category.Name;
                }

                var manufacturerName = await _localizedEntityService.GetLocalizedValueAsync(languageId, manufacturer.Id, "Manufacturer", "Name");
                if(string.IsNullOrWhiteSpace(manufacturerName))
                {
                    manufacturerName = manufacturer.Name;
                }

                var categorySlug = await _urlRecordService.GetActiveSlugAsync(category.Id, "Category", languageId);
                if (string.IsNullOrEmpty(categorySlug))
                {
                    categorySlug = await _urlRecordService.GetSeNameAsync(categoryName, true, false);
                }

                var manufacturerCategory = new ManufacturerCategory
                {
                    CategoryName = categoryName,
                    CategorySeoName = categorySlug,
                    ManufacturerName = manufacturerName,
                    ManufacturerSeoName = await _urlRecordService.GetActiveSlugAsync(manufacturer.Id, "Manufacturer", languageId)
                };                

                manufacturerCategories.Add(manufacturerCategory);
            }

            return manufacturerCategories;
        }

        public async Task<ManufacturerCategory> GetManufacturerCategoryAsync(int categoryId, int manufacturerId)
        {
            var manufacturerCategory = await _manufacturerCategoryRepository.Table
              .Where(mc => mc.CategoryId == categoryId && mc.ManufacturerId == manufacturerId)              
              .FirstOrDefaultAsync();

            return manufacturerCategory;
        }

        public async Task<IList<Category>> GetCategoriesByManufacturerAsync(int manufacturerId, int parentCategoryId = 0)
        {
            var productIds = await _productManufacturerRepository
                .Table
                .Where(pm => pm.ManufacturerId == manufacturerId)
                .Select(pm => pm.ProductId)
                .ToListAsync();

            var categoryIds = await _productCategoryRepository
                .Table
                .Where(pc => productIds.Contains(pc.ProductId))
                .Select(pc => pc.CategoryId)
                .Distinct()
                .ToListAsync();

            var categories = await _categoryRepository
                  .Table
                  .Where(c => categoryIds.Contains(c.Id) && c.ParentCategoryId == parentCategoryId && c.Deleted == false && c.IncludeInTopMenu && c.Published)
                  .ToListAsync();

            return categories;
        }

        public async Task<IList<Product>> GetProductsByCategoryAndManufacturerAsync(int manufacturerId, int categoryId)
        {
            // Retrieve product IDs associated with both the manufacturer and category
            var productIds = await (from pm in _productManufacturerRepository.Table
                                    join pc in _productCategoryRepository.Table on pm.ProductId equals pc.ProductId
                                    where pm.ManufacturerId == manufacturerId && pc.CategoryId == categoryId
                                    select pm.ProductId)
                                    .ToListAsync();

            // Fetch product details for the matched product IDs
            var products = await _productService.GetProductsByIdsAsync(productIds.ToArray());

            // Filter the products to include only published and non-deleted products
            products = products.Where(p => p.Published && !p.Deleted).ToList();

            return products;
        }
    }
}