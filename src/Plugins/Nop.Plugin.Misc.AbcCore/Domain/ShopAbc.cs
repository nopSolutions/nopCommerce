using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class ShopAbc : BaseEntity
    {
        public virtual int ShopId { get; set; }
        public virtual string AbcId { get; set; }
        public virtual string AbcEmail { get; set; }
        public virtual string ManagerEmail { get; set; }
    }
}
