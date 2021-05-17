using System;
using Nop.Data;
using Nop.Plugin.Demo.CreateRoom.Domains;

namespace Nop.Plugin.Demo.CreateRoom.Services
{
    internal class RoomService : IRoomService
    {
        private readonly IRepository<Room> _roomRepository;

        public RoomService(IRepository<Room> productViewTrackerRecordRepository)
        {
            _roomRepository = productViewTrackerRecordRepository;
        }

        public virtual void Log(Room record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            _roomRepository.InsertAsync(record);
        }
    }
}