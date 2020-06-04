using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺标签映射表
    /// </summary>
    public partial class SupplierShopTagMapping : BaseEntity
    {
        /// <summary>
        /// SupplierShopTag表主键
        /// </summary>
        public int SupplierShopTagId { get; set; }
        /// <summary>
        /// SupplierShop表主键
        /// </summary>
        public int SupplierShopId { get; set; }

    }
}
