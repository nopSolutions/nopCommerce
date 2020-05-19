using System;

namespace Nop.Core.Domain.Weixin
{
    [Serializable]
    /// <summary>
    /// Represents an WUserBaseInfo
    /// </summary>
    public partial class WUserBaseInfo
    {
        /// <summary>
        /// 用户OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// UnionId
        /// </summary>
        public string UnionId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        /// <returns></returns>
        public string NickName { get; set; }

        /// <summary>
        /// 头像完整路径
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        public bool Subscribe { get; set; }
        /// <summary>
        /// 关注时间
        /// </summary>
        public int SubscribeTime { get; set; }
        /// <summary>
        /// 取消关注时间
        /// </summary>
        public int UnSubscribeTime { get; set; }
    }
}
