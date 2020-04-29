namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeImageProductMapping
    /// </summary>
    public partial class WQrCodeImageProductMapping : BaseEntity
    {
        /// <summary>
        /// WQrCodeImage表的ID
        /// </summary>
        public int WQrCodeImageId { get; set; }
        /// <summary>
        /// 产品表ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
