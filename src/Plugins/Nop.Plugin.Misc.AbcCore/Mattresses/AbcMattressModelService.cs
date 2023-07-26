using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public class AbcMattressModelService : IAbcMattressModelService
    {
        private readonly IRepository<AbcMattressModel> _abcMattressModelRepository;

        public AbcMattressModelService(
            IRepository<AbcMattressModel> abcMattressModelRepository
        )
        {
            _abcMattressModelRepository = abcMattressModelRepository;
        }

        public AbcMattressModel GetAbcMattressModelByProductId(int productId)
        {
            return GetAllAbcMattressModels().Where(amm => amm.ProductId == productId).FirstOrDefault();
        }

        public IList<AbcMattressModel> GetAllAbcMattressModels()
        {
            return _abcMattressModelRepository.Table.ToList();
        }

        public async Task UpdateAbcMattressModelAsync(AbcMattressModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            await _abcMattressModelRepository.UpdateAsync(model);
        }

        public bool IsProductMattress(int productId)
        {
            return GetAllAbcMattressModels().Where(amm => amm.ProductId == productId).Any();
        }
    }
}
