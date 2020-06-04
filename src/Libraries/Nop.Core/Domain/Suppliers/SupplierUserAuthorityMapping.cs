using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 供应商员工绑定信息
    /// </summary>
    public partial class SupplierUserAuthorityMapping : BaseEntity
    {
        /// <summary>
        /// 【Supplier.Id】供应商ID
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 【SupplierShop.Id】供应商门店ID
        /// </summary>
        public int? SupplierShopId { get; set; }
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 是否财务管理员
        /// </summary>
        public bool FinancialManager { get; set; }
        /// <summary>
        /// 是否核销员
        /// </summary>
        public bool VerifyManager { get; set; }
        /// <summary>
        /// 是否预定订单确认员
        /// </summary>
        public bool OrderConfirmer { get; set; }
        /// <summary>
        /// 是否产品发布员
        /// </summary>
        public bool ProductPulisher { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
