using Nop.Core;

namespace AO.Services.Domain
{
    public partial class AOInvRack : BaseEntity
    {        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public int AOInvWareHouseId { get; set; }
    }
}
