using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public interface IAbcCategoryService : ICategoryService
    {
        Task<Category> GetCategoryByNameAsync(string name);
    }
}
