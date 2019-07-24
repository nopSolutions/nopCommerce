using System.Collections.Generic;
using Nop.Core.Domain.Security;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer role
    /// </summary>
    public partial class CustomerRole : BaseEntity
    {
        private ICollection<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappings;

        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as free shipping
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as tax exempt
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is system
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the customer role system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customers must change passwords after a specified time
        /// </summary>
        public bool EnablePasswordLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customers of this role have other tax display type chosen instead of the default one
        /// </summary>
        public bool OverrideTaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets identifier of the default tax display type (used only with "OverrideTaxDisplayType" enabled)
        /// </summary>
        public int DefaultTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a product identifier that is required by this customer role. 
        /// A customer is added to this customer role once a specified product is purchased.
        /// </summary>
        public int PurchasedWithProductId { get; set; }

        /// <summary>
        /// Gets or sets the permission record-customer role mappings
        /// </summary>
        public virtual ICollection<PermissionRecordCustomerRoleMapping> PermissionRecordCustomerRoleMappings
        {
            get => _permissionRecordCustomerRoleMappings ?? (_permissionRecordCustomerRoleMappings = new List<PermissionRecordCustomerRoleMapping>());
            protected set => _permissionRecordCustomerRoleMappings = value;
        }
    }
}