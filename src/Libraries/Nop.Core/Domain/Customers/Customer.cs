using System;
using System.Collections.Generic;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<CustomerContent> _customerContent;
        private ICollection<CustomerRole> _customerRoles;
        private ICollection<ShoppingCartItem> _shoppingCartItems;
        private ICollection<Order> _orders;
        private ICollection<RewardPointsHistory> _rewardPointsHistory;
        private ICollection<ReturnRequest> _returnRequests;
        private ICollection<Address> _addresses;
        private ICollection<ForumTopic> _forumTopics;
        private ICollection<ForumPost> _forumPosts;

        public Customer()
        {
            this.CustomerGuid = Guid.NewGuid();
            this.PasswordFormat = PasswordFormat.Clear;
        }

        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public virtual Guid CustomerGuid { get; set; }

        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual int PasswordFormatId { get; set; }
        public virtual PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }

        public virtual string PasswordSalt { get; set; }
        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public virtual string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public virtual int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public virtual bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a VAT number (including counry code)
        /// </summary>
        public virtual string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number status identifier
        /// </summary>
        public virtual int VatNumberStatusId { get; set; }

        /// <summary>
        /// Gets or sets the last payment method system name (selected one)
        /// </summary>
        public virtual string SelectedPaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the selected checkout attributes (serialized)
        /// </summary>
        public virtual string CheckoutAttributes { get; set; }

        /// <summary>
        /// Gets or sets the applied discount coupon code
        /// </summary>
        public virtual string DiscountCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the applied gift card coupon codes (serialized)
        /// </summary>
        public virtual string GiftCardCouponCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use reward points during checkout
        /// </summary>
        public virtual bool UseRewardPointsDuringCheckout { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public virtual string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public virtual int AffiliateId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public virtual bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public virtual string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public virtual DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime LastActivityDateUtc { get; set; }
        
        #region Custom properties

        /// <summary>
        /// Gets the VAT number status
        /// </summary>
        public virtual VatNumberStatus VatNumberStatus
        {
            get
            {
                return (VatNumberStatus)this.VatNumberStatusId;
            }
            set
            {
                this.VatNumberStatusId = (int)value;
            }
        }
        
        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        {
            get { return _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>()); }
            protected set { _externalAuthenticationRecords = value; }
        }

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<CustomerContent> CustomerContent
        {
            get { return _customerContent ?? (_customerContent = new List<CustomerContent>()); }
            protected set { _customerContent = value; }
        }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole>()); }
            protected set { _customerRoles = value; }
        }

        /// <summary>
        /// Gets or sets shopping cart items
        /// </summary>
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems
        {
            get { return _shoppingCartItems ?? (_shoppingCartItems = new List<ShoppingCartItem>()); }
            protected set { _shoppingCartItems = value; }            
        }

        /// <summary>
        /// Gets or sets orders
        /// </summary>
        public virtual ICollection<Order> Orders
        {
            get { return _orders ?? (_orders = new List<Order>()); }
            protected set { _orders = value; }            
        }

        /// <summary>
        /// Gets or sets reward points history
        /// </summary>
        public virtual ICollection<RewardPointsHistory> RewardPointsHistory
        {
            get { return _rewardPointsHistory ?? (_rewardPointsHistory = new List<RewardPointsHistory>()); }
            protected set { _rewardPointsHistory = value; }            
        }

        /// <summary>
        /// Gets or sets return request of this customer
        /// </summary>
        public virtual ICollection<ReturnRequest> ReturnRequests
        {
            get { return _returnRequests ?? (_returnRequests = new List<ReturnRequest>()); }
            protected set { _returnRequests = value; }            
        }
        
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
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }            
        }
        
        /// <summary>
        /// Gets or sets the created forum topics
        /// </summary>
        public virtual ICollection<ForumTopic> ForumTopics
        {
            get { return _forumTopics ?? (_forumTopics = new List<ForumTopic>()); }
            protected set { _forumTopics = value; }
        }

        /// <summary>
        /// Gets or sets the created forum posts
        /// </summary>
        public virtual ICollection<ForumPost> ForumPosts
        {
            get { return _forumPosts ?? (_forumPosts = new List<ForumPost>()); }
            protected set { _forumPosts = value; }
        }
        
        #endregion
    }
}