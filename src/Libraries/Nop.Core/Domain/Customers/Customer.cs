using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<Customer_CustomerRole_Mapping> _customerCustomerRoles;
        private ICollection<CustomerRole> _customerRolesonly;
        private ICollection<ShoppingCartItem> _shoppingCartItems;
        private ICollection<ReturnRequest> _returnRequests;
        private ICollection<CustomerAddresses> _customerAddresses;
        private ICollection<Address> _addressesonly;

        /// <summary>
        /// Ctor
        /// </summary>
        public Customer()
        {
            this.CustomerGuid = Guid.NewGuid();
            _customerAddresses = new ObservableCollection<CustomerAddresses>();
            _addressesonly = new ObservableCollection<Address>();
            ((ObservableCollection<Address>)_addressesonly).CollectionChanged += Customer_CollectionChanged;
            ((ObservableCollection<CustomerAddresses>)_customerAddresses).CollectionChanged += Customer_CollectionChanged1;
            _customerCustomerRoles = new ObservableCollection<Customer_CustomerRole_Mapping>();
            _customerRolesonly = new ObservableCollection<CustomerRole>();
            ((ObservableCollection<CustomerRole>)_customerRolesonly).CollectionChanged += Customer_CollectionChanged2; ;
            ((ObservableCollection<Customer_CustomerRole_Mapping>)_customerCustomerRoles).CollectionChanged += Customer_CollectionChanged3;
        }

        /// <summary>
        /// Gets or sets the customer GUID
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the email that should be re-validated. Used in scenarios when a customer is already registered and wants to change an email address.
        /// </summary>
        public string EmailToRevalidate { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier with which this customer is associated (maganer)
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this customer has some products in the shopping cart
        /// <remarks>The same as if we run this.ShoppingCartItems.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load "ShoppingCartItems" navigation property for each page load
        /// It's used only in a couple of places in the presenation layer
        /// </remarks>
        /// </summary>
        public bool HasShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is required to re-login
        /// </summary>
        public bool RequireReLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of failed login attempts (wrong password)
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which a customer cannot login (locked out)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        ///  Gets or sets the store identifier in which customer registered
        /// </summary>
        public int RegisteredInStoreId { get; set; }

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
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<Customer_CustomerRole_Mapping> CustomerCustomerRoles
        {
            get { return _customerCustomerRoles; }
            protected set { _customerCustomerRoles = value; }
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
        public virtual ICollection<CustomerAddresses> CustomerAddresses
        {
            get
            {
                return _customerAddresses;

            }
            protected set
            {
                _customerAddresses = value;
            }            
        }
        private bool internalmodify = false;
        private void Customer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (Address newitem in e.NewItems)
                {
                    var ca = new CustomerAddresses()
                    {
                        Address = newitem,
                        AddressId = newitem.Id,
                        Customer = this,
                        CustomerId = this.Id
                    };
                    internalmodify = true;
                    CustomerAddresses.Add(ca);
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Address olditem in e.OldItems)
                {
                    var item = ((List<CustomerAddresses>) CustomerAddresses).Find(p =>
                        p.AddressId == olditem.Id && p.CustomerId == this.Id);
                    internalmodify = true;
                    CustomerAddresses.Remove(item);
                    internalmodify = false;
                }
            }
        }

        private void Customer_CollectionChanged1(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (CustomerAddresses newitem in e.NewItems)
                {
                    internalmodify = true;
                    try
                    {
                        _addressesonly.Add(newitem.Address);
                    }
                    catch (Exception) { }
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (CustomerAddresses olditem in e.OldItems)
                {
                    var item = ((List<Address>) _addressesonly).Find(p => p.Id == olditem.AddressId);
                    internalmodify = true;
                    _addressesonly.Remove(item);
                    internalmodify = false;
                }
            }
        }


        public ICollection<Address> Addresses
        {

            get
            {
                if (_addressesonly.Contains(null) || (_addressesonly.Count != CustomerAddresses.Count))
                {
                    internalmodify = true;
                    _addressesonly.Clear();
                    foreach (var item in CustomerAddresses)
                    {
                        try
                        {
                            _addressesonly.Add(item.Address);
                        }
                        catch (Exception) { }
                    }
                    internalmodify = false;
                }
                return _addressesonly;
            }
        }

        private void Customer_CollectionChanged2(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (CustomerRole newitem in e.NewItems)
                {
                    var ca = new Customer_CustomerRole_Mapping()
                    {
                        CustomerRole = newitem,
                        CustomerRoleId = newitem.Id,
                        Customer = this,
                        CustomerId = this.Id
                    };
                    internalmodify = true;
                    CustomerCustomerRoles.Add(ca);
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (CustomerRole olditem in e.OldItems)
                {
                    var item = ((List<Customer_CustomerRole_Mapping>)CustomerCustomerRoles).Find(p =>
                        p.CustomerRoleId == olditem.Id && p.CustomerId == this.Id);
                    internalmodify = true;
                    CustomerCustomerRoles.Remove(item);
                    internalmodify = false;
                }
            }
        }

        private void Customer_CollectionChanged3(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (internalmodify == true) return;
            if (e.NewItems != null)
            {
                foreach (Customer_CustomerRole_Mapping newitem in e.NewItems)
                {
                    internalmodify = true;
                    try
                    {
                        _customerRolesonly.Add(newitem.CustomerRole);
                    }
                    catch (Exception) { }
                    internalmodify = false;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Customer_CustomerRole_Mapping olditem in e.OldItems)
                {
                    var item = ((List<CustomerRole>)_customerRolesonly).Find(p => p.Id == olditem.CustomerRoleId);
                    internalmodify = true;
                    _customerRolesonly.Remove(item);
                    internalmodify = false;
                }
            }
        }

        public ICollection<CustomerRole> CustomerRoles
        {
            get
            {
                if (_customerRolesonly.Contains(null) || (_customerRolesonly.Count !=CustomerCustomerRoles.Count))
                {
                    //regenerate
                    internalmodify = true;
                    _customerRolesonly.Clear();
                    foreach (var item in CustomerCustomerRoles)
                    {
                        _customerRolesonly.Add(item.CustomerRole);
                    }
                    internalmodify = false;
                }
                return _customerRolesonly;
            }
        }

        #endregion
    }
}