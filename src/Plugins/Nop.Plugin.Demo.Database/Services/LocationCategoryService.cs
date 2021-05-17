using System;
using Nop.Data;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Services
{
    internal class LocationCategoryService : ILocationCategoryService
    {
        private readonly IRepository<LocationCategory> _locationCategoryRepository;

        public LocationCategoryService(IRepository<LocationCategory> productViewTrackerRecordRepository)
        {
            _locationCategoryRepository = productViewTrackerRecordRepository;
        }

        public virtual void Log(LocationCategory record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _locationCategoryRepository.InsertAsync(record);
        }
    }
}