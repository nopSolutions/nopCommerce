using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressModelGiftMappingService
    {
        IList<AbcMattressModelGiftMapping> GetAbcMattressModelGiftMappingsByModelId(int modelId);
    }
}
