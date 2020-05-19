namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a RoleType
    /// </summary>
    public enum WRoleType : byte
    {
        /// <summary>
        /// 常规游客身份（默认）
        /// </summary>
        Visitor = 0,
        /// <summary>
        /// 销售员
        /// </summary>
        Saler = 1,
        /// <summary>
        /// 核销员
        /// </summary>
        Verifier = 2,
        /// <summary>
        /// 商家
        /// </summary>
        Store = 2,
    }
}
