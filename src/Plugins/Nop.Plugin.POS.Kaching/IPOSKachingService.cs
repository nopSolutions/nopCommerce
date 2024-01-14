using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching
{
    public interface IPOSKachingService
    {
        Task<string> BuildJSONStringAsync(Product product);

        string BuildJSONStringForCategory(string productCategory);

        Task SaveProductAsync(string json);

        Task SaveProductCategoryAsync(string json);

        Task<bool> TestConnection();

        Task DeleteProductAsync(string[] ids);

        //string BuildJSONStringForDeletion(List<string> ids);
    }
}