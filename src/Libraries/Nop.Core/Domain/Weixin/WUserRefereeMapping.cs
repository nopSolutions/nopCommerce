namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an UserRefereeMapping
    /// </summary>
    public partial class WUserRefereeMapping : BaseEntity
    {
        /// <summary>
        /// 用户标识Openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 推荐用户标识ID
        /// </summary>
        /// <returns></returns>
        public string OpenIdReferee { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
