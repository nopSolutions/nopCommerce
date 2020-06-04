using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺标签表（主营产品标签，类别标签）等
    /// </summary>
    public partial class SupplierShopTag : BaseEntity
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        ///  删除
        /// </summary>
        public bool Deleted { get; set; }
    }
}
