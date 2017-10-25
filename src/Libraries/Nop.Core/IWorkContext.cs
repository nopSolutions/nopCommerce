using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;

namespace Nop.Core
{
    /// <summary>
    /// 定义了工作上下文接口
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// 当前用户
        /// Gets or sets the current customer
        /// </summary>
        Customer CurrentCustomer { get; set; }

        /// <summary>
        /// 原始用户？
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// 当前供应商
        /// Gets or sets the current vendor (logged-in manager)
        /// </summary>
        Vendor CurrentVendor { get; }

        /// <summary>
        /// 当前用户的工作语言
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// 当前用户的工作币种
        /// Get or set current user working currency
        /// </summary>
        Currency WorkingCurrency { get; set; }

        /// <summary>
        /// 税的显示方式
        /// Get or set current tax display type
        /// </summary>
        TaxDisplayType TaxDisplayType { get; set; }

        /// <summary>
        /// 是否是管理员
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
