using Nop.Core;

namespace AO.Services.Domain
{
    public partial class AOInvProductRelation : BaseEntity
    {
        public int AOInvPositionId { get; set; }

        public int ProductId { get; set; }
    }
}
