using Nop.Core;

namespace AO.Services.Domain
{
    public partial class AOInvPosition : BaseEntity
    {
        public string Name { get; set; }

        public int AOInvShelfId { get; set; }
    }
}