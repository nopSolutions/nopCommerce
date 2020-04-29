namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeChannel
    /// </summary>
    public partial class WQrCodeChannel : BaseEntity
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态位，预留
        /// </summary>
        /// <returns></returns>
        public byte Status { get; set; }
        /// <summary>
        /// 删除标志，不允许直接删除
        /// </summary>
        /// <returns></returns>
        public bool Deleted { get; set; }
    }
}
