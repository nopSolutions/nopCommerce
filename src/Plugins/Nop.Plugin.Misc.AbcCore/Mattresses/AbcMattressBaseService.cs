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
    public class AbcMattressBaseService : IAbcMattressBaseService
    {
        private readonly IRepository<AbcMattressBase> _abcMattressBaseRepository;

        private readonly IAbcMattressEntryService _abcMattressEntryService;
        private readonly IAbcMattressPackageService _abcMattressPackageService;

        public AbcMattressBaseService(
            IRepository<AbcMattressBase> abcMattressBaseRepository,
            IAbcMattressEntryService abcMattressEntryService,
            IAbcMattressPackageService abcMattressPackageService
        )
        {
            _abcMattressBaseRepository = abcMattressBaseRepository;
            _abcMattressEntryService = abcMattressEntryService;
            _abcMattressPackageService = abcMattressPackageService;
        }

        public IList<AbcMattressBase> GetAbcMattressBasesByModelId(int modelId)
        {
            var entries = _abcMattressEntryService.GetAbcMattressEntriesByModelId(modelId);
            var packages = _abcMattressPackageService.GetAbcMattressPackagesByEntryIds(entries.Select(e => e.Id));
            var baseIds = packages.Select(p => p.AbcMattressBaseId);

            return _abcMattressBaseRepository.Table
                                             .Where(amb => baseIds.Contains(amb.Id) &&
                                                    !(amb.Name == null || amb.Name.Trim() == string.Empty))
                                             .ToList();
        }

        public IList<AbcMattressBase> GetAbcMattressBasesByEntryId(int entryId)
        {
            var packages = _abcMattressPackageService.GetAbcMattressPackagesByEntryIds(new int[] { entryId });
            var baseIds = packages.Select(p => p.AbcMattressBaseId);

            return _abcMattressBaseRepository.Table
                                             .Where(amb => baseIds.Contains(amb.Id) &&
                                                    !(amb.Name == null || amb.Name.Trim() == string.Empty))
                                             .ToList();
        }
    }
}
