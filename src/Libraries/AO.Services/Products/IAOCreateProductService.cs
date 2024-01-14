using AO.Services.Products.Models;
using System.Threading.Tasks;

namespace AO.Services.Products
{
    public interface IAOCreateProductService
    {
        Task<ProductDto> CreateProductAsync(VariantData variantData, string orgItemNumber, string updaterName);

        Task<ProductDto> GetProductAsync(string frilivSKU);

        Task CreateVariantAsync(VariantData variantData);
    }
}