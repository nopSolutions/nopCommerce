namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WLocation
    /// </summary>
    public partial class WLocation : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public decimal Latitude { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public decimal Longitude { get; set; }
        /// <summary>
        /// 地理位置精度
        /// </summary>
        public decimal Precision { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }

    }
}
