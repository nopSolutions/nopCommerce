using Nop.Core;

namespace AO.Services.Domain
{
    public partial class AOInvShelf : BaseEntity
    {
        public string Name { get; set; }        

        public int AOInvRackId { get; set; }
    }
}
