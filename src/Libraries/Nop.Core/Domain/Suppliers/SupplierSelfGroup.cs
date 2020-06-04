using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺信息分组管理（公用）
    /// </summary>
    public partial class SupplierSelfGroup : BaseEntity
    {
        /// <summary>
        /// 【Supplier.Id】供应商ID
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态（保留字段，是否通过审核等）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
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
