namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 促销活动方式使用范围
    /// </summary>
    public enum PromotionUseScopeType : byte
    {
        /// <summary>
        /// 全网
        /// </summary>
        All = 0,
        /// <summary>
        /// 跨站点=全网
        /// </summary>
        CrossStores = 1,

        /// <summary>
        /// 指定站点内
        /// </summary>
        InStroes = 2,

        /// <summary>
        /// 类别别内
        /// </summary>
        InCategories = 3,

        /// <summary>
        /// 供应商内
        /// </summary>
        InSuppliers = 4,

        /// <summary>
        /// 店铺内
        /// </summary>
        InShops = 5,

        /// <summary>
        /// 指定商品
        /// </summary>
        InProducts = 6,
    }
}
