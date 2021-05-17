using System;
using Nop.Data;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Services
{
    internal class LocationService : ILocationService
    {
        private readonly IRepository<Location> _locationRepository;

        public LocationService(IRepository<Location> productViewTrackerRecordRepository)
        {
            _locationRepository = productViewTrackerRecordRepository;
        }

        public virtual void Log(Location record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _locationRepository.InsertAsync(record);
        }
    }
}