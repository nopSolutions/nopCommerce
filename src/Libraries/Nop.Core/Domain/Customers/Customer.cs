//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        public Customer() {
            this.Addresses = new List<Address>();
        }
        
        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public int? CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the applied discount coupon code
        /// </summary>
        public string DiscountCouponCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime? LastActivityDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<CustomerContent> CustomerContent { get; set; }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> CustomerRoles { get; set; }

        /// <summary>
        /// Gets or sets customer attributes
        /// </summary>
        public virtual ICollection<CustomerAttribute> CustomerAttributes { get; set; }

        /// <summary>
        /// Default billing address
        /// </summary>
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Default shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses { get; set; }

        #region Methods

        public virtual void AddAddress(Address address) {
            if (!this.Addresses.Contains(address))
                this.Addresses.Add(address);
        }

        public virtual void RemoveAddress(Address address) {
            if (this.Addresses.Contains(address)) {
                if (this.BillingAddress == address) this.BillingAddress = null;
                if (this.ShippingAddress == address) this.ShippingAddress = null;

                this.Addresses.Remove(address);
            }
        }

        public virtual void SetBillingAddress(Address address) {
            if (this.Addresses.Contains(address))
                this.BillingAddress = address;
        }

        public virtual void SetShippingAddress(Address address) {
            if (this.Addresses.Contains(address))
                this.ShippingAddress = address;
        }

        #endregion
    }
}