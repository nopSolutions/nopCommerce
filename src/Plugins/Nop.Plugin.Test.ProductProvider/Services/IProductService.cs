using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Test.ProductProvider.Models;

namespace Nop.Plugin.Test.ProductProvider.Services;

public interface IProductService
{
    Task GetAllProducts();
    ExternalProductModel GetProductById(int id);
}