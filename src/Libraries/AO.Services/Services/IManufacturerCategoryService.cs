using AO.Services.Models;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IManufacturerCategoryService
    {
        Task<IList<ManufacturerCategory>> GetManufacturerCategoriesAsync(IList<Category> categories, Manufacturer manufacturer, int languageId);

        Task<IList<Category>> GetCategoriesByManufacturerAsync(int manufacturerId, int parentCategoryId = 0);

        Task<IList<Product>> GetProductsByCategoryAndManufacturerAsync(int manufacturerId, int categoryId);

        Task<ManufacturerCategory> GetManufacturerCategoryAsync(int categoryId, int manufacturerId);
    }
}