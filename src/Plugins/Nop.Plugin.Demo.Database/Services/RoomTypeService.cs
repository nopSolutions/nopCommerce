using System;
using Nop.Data;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Services
{
    internal class RoomTypeService : IRoomTypeService
    {
        private readonly IRepository<RoomType> _roomTypeRepository;

        public RoomTypeService(IRepository<RoomType> productViewTrackerRecordRepository)
        {
            _roomTypeRepository = productViewTrackerRecordRepository;
        }

        public virtual void Log(RoomType record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _roomTypeRepository.InsertAsync(record);
        }
    }
}