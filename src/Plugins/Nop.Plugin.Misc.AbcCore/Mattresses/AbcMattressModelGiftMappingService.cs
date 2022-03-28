using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public class AbcMattressModelGiftMappingService : IAbcMattressModelGiftMappingService
    {
        private readonly IRepository<AbcMattressModelGiftMapping> _abcMattressModelGiftMappingRepository;

        public AbcMattressModelGiftMappingService(
            IRepository<AbcMattressModelGiftMapping> abcMattressModelGiftMappingRepository
        )
        {
            _abcMattressModelGiftMappingRepository = abcMattressModelGiftMappingRepository;
        }

        public IList<AbcMattressModelGiftMapping> GetAbcMattressModelGiftMappingsByModelId(int modelId)
        {
            return _abcMattressModelGiftMappingRepository.Table.Where(ammgm => ammgm.AbcMattressModelId == modelId).ToList();
        }
    }
}
