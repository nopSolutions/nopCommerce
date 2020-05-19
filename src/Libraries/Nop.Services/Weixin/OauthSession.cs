using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// 保存weixin oauth 值
    /// </summary>
    [Serializable]
    public partial class OauthSession
    {
        public OauthSession()
        {
            UserBaseInfo = new WUserBaseInfo();
        }
        /// <summary>
        /// 原始返回页面，非微信的回调页面(备用)
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 有效期2小时
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 有效期30天
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// CreatTime 最后创建/更新时间
        /// </summary>
        public int CreatTime { get; set; }

        public WUserBaseInfo UserBaseInfo { get; set; }

    }
}
