using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressGiftService
    {
        IList<AbcMattressGift> GetAbcMattressGiftsByModelId(int modelId);

        AbcMattressGift GetAbcMattressGiftByDescription(string description);
    }
}
