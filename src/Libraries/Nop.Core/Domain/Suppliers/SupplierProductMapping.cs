using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 产品绑定供应商（或商家）
    /// </summary>
    public partial class SupplierProductMapping : BaseEntity
    {
        /// <summary>
        /// 【Supplier.Id】
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 【Product.Id】
        /// </summary>
        public int ProductId { get; set; }

        public int SupplierShopId { get; set; }

        public int DisplayOrder { get; set; }
    }
}
