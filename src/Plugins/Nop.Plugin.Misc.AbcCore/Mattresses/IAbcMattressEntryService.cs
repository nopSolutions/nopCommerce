using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressEntryService
    {
        IList<AbcMattressEntry> GetAllAbcMattressEntries();
        IList<AbcMattressEntry> GetAbcMattressEntriesByModelId(int modelId);
    }
}
