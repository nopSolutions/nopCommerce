using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Test.ProductProvider.Models;

namespace Nop.Plugin.Test.ProductProvider.Services;

public interface IProductService
{
    // Task<IEnumerable<int>> GetAllProducts();
    Task GetProductDetails(int id);
}