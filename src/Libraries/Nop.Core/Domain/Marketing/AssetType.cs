namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// Represents 资产类型
    /// </summary>
    public enum AssetType : byte
    {
        /// <summary>
        /// 现金
        /// </summary>
        Amount = 1,
        /// <summary>
        /// 积分
        /// </summary>
        Point = 2,
        /// <summary>
        /// 虚拟币
        /// </summary>
        VirtualCurrency = 3,
        /// <summary>
        /// 实物卡
        /// </summary>
        PhysicalCard = 4,
        /// <summary>
        /// 虚拟卡
        /// </summary>
        VirtualCard = 5,
        /// <summary>
        /// 抵用券/兑换券
        /// </summary>
        Voucher = 6,
        /// <summary>
        /// 折扣券
        /// </summary>
        Coupon = 7,
        /// <summary>
        /// 经验值
        /// </summary>
        ExperiencePoint = 8
    }
}
