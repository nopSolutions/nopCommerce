namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WConfig
    /// </summary>
    public partial class WConfig : BaseEntity
    {
        /// <summary>
        /// 微信公众号原始ID，用于区分不同平台用户
        /// </summary>
        public string OriginalId { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        /// <returns></returns>
        public string SystemName { get; set; }
        /// <summary>
        /// 对公众号备注，便于后台管理
        /// </summary>
        /// <returns></returns>
        public string Remark { get; set; }
        /// <summary>
        /// 微信公众号appid
        /// </summary>
        /// <returns></returns>
        public string WeixinAppId { get; set; }
        /// <summary>
        /// 微信公众号appsecret
        /// </summary>
        /// <returns></returns>
        public string WeixinAppSecret { get; set; }
        /// <summary>
        /// 微信公众号token
        /// </summary>
        /// <returns></returns>
        public string Token { get; set; }
        /// <summary>
        /// 微信公众号EncodingAESKey
        /// </summary>
        /// <returns></returns>
        public string EncodingAESKey { get; set; }
        /// <summary>
        /// 微信小程序appid
        /// </summary>
        /// <returns></returns>
        public string WxOpenAppId { get; set; }
        /// <summary>
        /// 微信小程序appsecret
        /// </summary>
        /// <returns></returns>
        public string WxOpenAppSecret { get; set; }
        /// <summary>
        /// 微信小程序token
        /// </summary>
        /// <returns></returns>
        public string WxOpenToken { get; set; }
        /// <summary>
        /// 微信小程序EncodingAESKey
        /// </summary>
        /// <returns></returns>
        public string WxOpenEncodingAESKey { get; set; }
        /// <summary>
        /// 状态，预留
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 发布中
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Deleted { get; set; }

    }
}
