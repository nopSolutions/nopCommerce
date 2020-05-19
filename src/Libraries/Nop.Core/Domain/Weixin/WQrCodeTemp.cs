namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeTemp
    /// </summary>
    public partial class WQrCodeTemp : BaseEntity
    {
        /// <summary>
        /// 用户OpenId，不能为空
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 背景图ID，不使用背景图，值为0
        /// </summary>
        public int QrCodeImageId { get; set; }
        /// <summary>
        /// 过期时间 = 创建时间 + 过期秒数
        /// </summary>
        public int ExpireTime { get; set; }
        /// <summary>
        /// 获取的二维码ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 二维码图片解析后的地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
