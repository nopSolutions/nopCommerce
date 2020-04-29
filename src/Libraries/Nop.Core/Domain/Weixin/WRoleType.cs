namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a RoleType
    /// </summary>
    public enum WRoleType : byte
    {
        //常规游客身份（默认）
        Visitor = 0,
        //销售员
        Saler = 1,
        //商家
        Store = 2,
    }
}
