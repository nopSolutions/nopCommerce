using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// 定义一个权限记录实体
    /// Represents a permission record
    /// </summary>
    public partial class PermissionRecord : BaseEntity
    {
        private ICollection<CustomerRole> _customerRoles;

        /// <summary>
        /// 权限名称
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 系统名称
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }
        
        /// <summary>
        /// 类别
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// 用户角色
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<CustomerRole> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole>()); }
            protected set { _customerRoles = value; }
        }   
    }
}
