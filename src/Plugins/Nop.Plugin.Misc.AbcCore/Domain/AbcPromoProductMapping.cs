using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public class AbcPromoProductMapping : BaseEntity
    {
        public int AbcPromoId { get; set; }
        public int ProductId { get; set; }
    }
}
