using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public class AbcMattressPackageService : IAbcMattressPackageService
    {
        private readonly IRepository<AbcMattressPackage> _abcMattressPackageRepository;

        public AbcMattressPackageService(
            IRepository<AbcMattressPackage> abcMattressPackageRepository
        )
        {
            _abcMattressPackageRepository = abcMattressPackageRepository;
        }

        public AbcMattressPackage GetAbcMattressPackageByEntryIdAndBaseId(int entryId, int baseId)
        {
            return _abcMattressPackageRepository.Table
                                         .Where(p => p.AbcMattressBaseId == baseId &&
                                                     p.AbcMattressEntryId == entryId &&
                                                     p.Price > 0M)
                                         .FirstOrDefault();
        }

        public IList<AbcMattressPackage> GetAbcMattressPackagesByEntryIds(IEnumerable<int> entryIds)
        {
            return _abcMattressPackageRepository.Table.Where(amp => entryIds.Contains(amp.AbcMattressEntryId)).ToList();
        }

        public IList<AbcMattressPackage> GetAllAbcMattressPackages()
        {
            return _abcMattressPackageRepository.Table.ToList();
        }
    }
}
