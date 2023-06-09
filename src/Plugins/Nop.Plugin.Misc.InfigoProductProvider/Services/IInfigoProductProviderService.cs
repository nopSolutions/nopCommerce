using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public interface IInfigoProductProviderService
{
    public Task GetApiProducts();
    public Task<Product> GetProductByExternalId(int externalId);
}