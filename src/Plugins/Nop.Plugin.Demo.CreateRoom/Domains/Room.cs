using Nop.Core;

namespace Nop.Plugin.Demo.CreateRoom.Domains
{
    public class Room : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int MaximumDate { get; set; }
        public int LocationId { get; set; }
        public int RoomTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}