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
    public class AbcMattressEntryService : IAbcMattressEntryService
    {
        private readonly IRepository<AbcMattressEntry> _abcMattressEntryRepository;

        public AbcMattressEntryService(
            IRepository<AbcMattressEntry> abcMattressEntryRepository
        )
        {
            _abcMattressEntryRepository = abcMattressEntryRepository;
        }

        public IList<AbcMattressEntry> GetAbcMattressEntriesByModelId(int modelId)
        {
            return _abcMattressEntryRepository.Table
                .Where(ame => ame.AbcMattressModelId == modelId && ame.Price > 0M)
                .ToList();
        }

        public IList<AbcMattressEntry> GetAllAbcMattressEntries()
        {
            return _abcMattressEntryRepository.Table.ToList();
        }
    }
}
