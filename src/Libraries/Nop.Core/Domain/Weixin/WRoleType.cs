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
        /// 是否财务管理员
        /// </summary>
        FinancialManager = 2,
        /// <summary>
        /// 是否核销员
        /// </summary>
        VerifyManager = 3,
        /// <summary>
        /// 是否预定订单确认员
        /// </summary>
        OrderConfirmer = 4,
        /// <summary>
        /// 产品发布员
        /// </summary>
        ProductPulisher = 5,
        /// <summary>
        /// 供应商家
        /// </summary>
        Store = 6,
    }
}
