namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WUserUserSysTagMapping
    /// </summary>
    public partial class WUserUserSysTagMapping : BaseEntity
    {
        public int WUserId { get; set; }
        public int WUserSysTagId { get; set; }
    }
}
