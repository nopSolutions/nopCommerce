using System;

namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WQrCodeLimitUserMapping
    /// </summary>
    public partial class WQrCodeLimitUserMapping : BaseEntity
    {
        /// <summary>
        /// 绑定到用户的ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 永久二维码数字ID
        /// </summary>
        public int QrCodeLimitId { get; set; }
        /// <summary>
        /// 显示绑定的用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 联系电话信息
        /// </summary>
        public string TelNumber { get; set; }
        /// <summary>
        /// 地址信息
        /// </summary>
        public string AddressInfo { get; set; }
        /// <summary>
        /// 绑定用户的过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 发布标志
        /// </summary>
        public bool Published { get; set; }

    }
}
