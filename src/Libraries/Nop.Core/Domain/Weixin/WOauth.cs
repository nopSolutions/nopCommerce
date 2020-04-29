namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WOauth
    /// </summary>
    public partial class WOauth : BaseEntity
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 网页授权接口调用凭证
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 用户刷新access_token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 授权范围
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
