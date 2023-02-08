using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressModelService
    {
        IList<AbcMattressModel> GetAllAbcMattressModels();
        Task UpdateAbcMattressModelAsync(AbcMattressModel model);
        AbcMattressModel GetAbcMattressModelByProductId(int productId);
        bool IsProductMattress(int productId);
    }
}
