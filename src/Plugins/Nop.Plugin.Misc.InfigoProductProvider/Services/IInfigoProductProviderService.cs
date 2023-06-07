using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public interface IInfigoProductProviderService
{
    public Task<List<int>> GetAllProductsIds();
    public Task<ProductModel> GetProductById(int id);
}