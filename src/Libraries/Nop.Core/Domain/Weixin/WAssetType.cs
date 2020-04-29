namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a AssetType
    /// </summary>
    public enum WAssetType : byte
    {
        //资产类型
        //现金
        Amount = 1,
        //积分
        Point = 2,
        //虚拟币
        VirtualCurrency = 3,
        //实物卡
        PhysicalCard = 4,
        //虚拟卡
        VirtualCard = 5,
    }
}
