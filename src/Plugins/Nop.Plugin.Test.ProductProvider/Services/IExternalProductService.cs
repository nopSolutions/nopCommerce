using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Test.ProductProvider.Models;

namespace Nop.Plugin.Test.ProductProvider.Services;

public interface IExternalProductService
{
    Task<IEnumerable<int>> GetAllProducts();
    Task<ExternalProductModel> GetProductDetails(int id); 
    Task SyncProducts();
}