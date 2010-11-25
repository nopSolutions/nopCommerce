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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.CustomerManagement
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        #region Fields
        private List<CustomerAttribute> _customerAttributesCache = null;
        private List<CustomerRole> _customerRolesCache = null;
        private Address _billingAddressCache = null;
        private Address _shippingAddressCache = null;
        private List<RewardPointsHistory> _rewardPointsHistoryCache = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the Customer class
        /// </summary>
        public Customer()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets cached values for an instance
        /// </summary>
        public void ResetCachedValues()
        {
            _customerAttributesCache = null;
            _customerRolesCache = null;
            _billingAddressCache = null;
            _shippingAddressCache = null;
            _rewardPointsHistoryCache = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

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
        /// Gets or sets the password hash
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the salt key
        /// </summary>
        public string SaltKey { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier
        /// </summary>
        public int BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the shipping address identifier
        /// </summary>
        public int ShippingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the last payment method identifier
        /// </summary>
        public int LastPaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets the last applied coupon code
        /// </summary>
        public string LastAppliedCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the applied gift card coupon code
        /// </summary>
        public string GiftCardCouponCodes { get; set; }

        /// <summary>
        /// Gets or sets the selected checkout attributes
        /// </summary>
        public string CheckoutAttributes { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the tax display type identifier
        /// </summary>
        public int TaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is administrator
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is guest
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is forum moderator
        /// </summary>
        public bool IsForumModerator { get; set; }

        /// <summary>
        /// Gets or sets the forum post count
        /// </summary>
        public int TotalForumPosts { get; set; }

        /// <summary>
        /// Gets or sets the signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of customer registration
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the avatar identifier
        /// </summary>
        public int AvatarId { get; set; }
        
        /// <summary>
        /// Gets or sets the date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the last billing address
        /// </summary>
        public Affiliate Affiliate
        {
            get
            {
                return IoC.Resolve<IAffiliateService>().GetAffiliateById(this.AffiliateId);
            }
        }

        /// <summary>
        /// Gets the customer attributes
        /// </summary>
        public List<CustomerAttribute> CustomerAttributes
        {
            get
            {
                if (_customerAttributesCache == null)
                    _customerAttributesCache = IoC.Resolve<ICustomerService>().GetCustomerAttributesByCustomerId(this.CustomerId);

                return _customerAttributesCache;
            }
        }

        /// <summary>
        /// Gets the customer roles
        /// </summary>
        public List<CustomerRole> CustomerRoles
        {
            get
            {
                if (_customerRolesCache == null)
                    _customerRolesCache = IoC.Resolve<ICustomerService>().GetCustomerRolesByCustomerId(this.CustomerId);

                return _customerRolesCache;
            }
        }

        /// <summary>
        /// Gets the last billing address
        /// </summary>
        public Address BillingAddress
        {
            get
            {
                if (_billingAddressCache == null)
                    _billingAddressCache = IoC.Resolve<ICustomerService>().GetAddressById(this.BillingAddressId);

                return _billingAddressCache;
            }
        }

        /// <summary>
        /// Gets the last shipping address
        /// </summary>
        public Address ShippingAddress
        {
            get
            {
                if (_shippingAddressCache == null)
                    _shippingAddressCache = IoC.Resolve<ICustomerService>().GetAddressById(this.ShippingAddressId);

                return _shippingAddressCache;
            }
        }

        /// <summary>
        /// Gets the language
        /// </summary>
        public Language Language
        {
            get
            {
                return IoC.Resolve<ILanguageService>().GetLanguageById(this.LanguageId);
            }
        }

        /// <summary>
        /// Gets the currency
        /// </summary>
        public Currency Currency
        {
            get
            {
                return IoC.Resolve<ICurrencyService>().GetCurrencyById(this.CurrencyId);
            }
        }

        /// <summary>
        /// Gets the billing addresses
        /// </summary>
        public List<Address> BillingAddresses
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetAddressesByCustomerId(this.CustomerId, true);
            }
        }

        /// <summary>
        /// Gets the shipping addresses
        /// </summary>
        public List<Address> ShippingAddresses
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetAddressesByCustomerId(this.CustomerId, false);
            }
        }

        /// <summary>
        /// Gets the orders
        /// </summary>
        public List<Order> Orders
        {
            get
            {
                return IoC.Resolve<IOrderService>().GetOrdersByCustomerId(this.CustomerId);
            }
        }

        /// <summary>
        /// Gets the tax display type
        /// </summary>
        public TaxDisplayTypeEnum TaxDisplayType
        {
            get
            {
                return (TaxDisplayTypeEnum)this.TaxDisplayTypeId;
            }
            set
            {
                this.TaxDisplayTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the avatar
        /// </summary>
        public Picture Avatar
        {
            get
            {
                return IoC.Resolve<IPictureService>().GetPictureById(this.AvatarId);
            }
        }

        /// <summary>
        /// Gets the last payment method
        /// </summary>
        public PaymentMethod LastPaymentMethod
        {
            get
            {
                return IoC.Resolve<IPaymentService>().GetPaymentMethodById(this.LastPaymentMethodId);
            }
        }

        /// <summary>
        /// Gets or sets the last shipping option
        /// </summary>
        public ShippingOption LastShippingOption
        {
            get
            {
                ShippingOption shippingOption = null;
                CustomerAttribute lastShippingOptionAttr = this.CustomerAttributes.FindAttribute("LastShippingOption", this.CustomerId);
                if (lastShippingOptionAttr != null && !String.IsNullOrEmpty(lastShippingOptionAttr.Value))
                {
                    try
                    {
                        using (TextReader tr = new StringReader(lastShippingOptionAttr.Value))
                        {
                            XmlSerializer xmlS = new XmlSerializer(typeof(ShippingOption));
                            shippingOption = (ShippingOption)xmlS.Deserialize(tr);
                        }
                    }
                    catch
                    {
                        //xml error
                    }
                }
                return shippingOption;
            }
            set
            {
                CustomerAttribute lastShippingOptionAttr = this.CustomerAttributes.FindAttribute("LastShippingOption", this.CustomerId);
                if (value != null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (TextWriter tw = new StringWriter(sb))
                    {
                        XmlSerializer xmlS = new XmlSerializer(typeof(ShippingOption));
                        xmlS.Serialize(tw, value);
                        string serialized = sb.ToString();
                        if (lastShippingOptionAttr != null)
                        {
                            lastShippingOptionAttr.Value = serialized;
                            IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(lastShippingOptionAttr);
                        }
                        else
                        {
                            lastShippingOptionAttr = new CustomerAttribute()
                            {
                                CustomerId = this.CustomerId,
                                Key = "LastShippingOption",
                                Value = serialized
                            };
                            IoC.Resolve<ICustomerService>().InsertCustomerAttribute(lastShippingOptionAttr);
                        }
                    }
                }
                else
                {
                    if (lastShippingOptionAttr != null)
                        IoC.Resolve<ICustomerService>().DeleteCustomerAttribute(lastShippingOptionAttr.CustomerAttributeId);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets the customer full name
        /// </summary>
        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(this.FirstName))
                    return this.LastName;
                else
                    return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        /// <summary>
        /// Gets or sets the gender
        /// </summary>
        public string Gender
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute genderAttr = customerAttributes.FindAttribute("Gender", this.CustomerId);
                if (genderAttr != null)
                    return genderAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute genderAttr = customerAttributes.FindAttribute("Gender", this.CustomerId);
                if (genderAttr != null)
                {
                    genderAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(genderAttr);
                }
                else
                {
                    genderAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "Gender",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(genderAttr);
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public string FirstName
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute firstNameAttr = customerAttributes.FindAttribute("FirstName", this.CustomerId);
                if (firstNameAttr != null)
                    return firstNameAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute firstNameAttr = customerAttributes.FindAttribute("FirstName", this.CustomerId);
                if (firstNameAttr != null)
                {
                    firstNameAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(firstNameAttr);
                }
                else
                {
                    firstNameAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "FirstName",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(firstNameAttr);
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public string LastName
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute lastNameAttr = customerAttributes.FindAttribute("LastName", this.CustomerId);
                if (lastNameAttr != null)
                    return lastNameAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute lastNameAttr = customerAttributes.FindAttribute("LastName", this.CustomerId);
                if (lastNameAttr != null)
                {
                    lastNameAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(lastNameAttr);
                }
                else
                {
                    lastNameAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "LastName",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(lastNameAttr);
                }

                ResetCachedValues();
            }
        }
        
        /// <summary>
        /// Gets or sets the company
        /// </summary>
        public string Company
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute companyAttr = customerAttributes.FindAttribute("Company", this.CustomerId);
                if (companyAttr != null)
                    return companyAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute companyAttr = customerAttributes.FindAttribute("Company", this.CustomerId);
                if (companyAttr != null)
                {
                    companyAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(companyAttr);
                }
                else
                {
                    companyAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "Company",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(companyAttr);
                }
                
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the VAT number (the European Union Value Added Tax)
        /// </summary>
        public string VatNumber
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute vatNumberAttr = customerAttributes.FindAttribute("VatNumber", this.CustomerId);
                if (vatNumberAttr != null)
                    return vatNumberAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute vatNumberAttr = customerAttributes.FindAttribute("VatNumber", this.CustomerId);
                if (vatNumberAttr != null)
                {
                    vatNumberAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(vatNumberAttr);
                }
                else
                {
                    vatNumberAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "VatNumber",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(vatNumberAttr);
                }
                
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the VAT number status
        /// </summary>
        public VatNumberStatusEnum VatNumberStatus
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute vatNumberStatusAttr = customerAttributes.FindAttribute("VatNumberStatus", this.CustomerId);
                if (vatNumberStatusAttr != null)
                {
                    int _vatNumberStatusId = 0;
                    int.TryParse(vatNumberStatusAttr.Value, out _vatNumberStatusId);
                    return (VatNumberStatusEnum)_vatNumberStatusId;
                }
                else
                    return VatNumberStatusEnum.Empty;
            }
            set
            {
                int vatNumberStatusId = (int)value;
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute vatNumberStatusAttr = customerAttributes.FindAttribute("VatNumberStatus", this.CustomerId);
                if (vatNumberStatusAttr != null)
                {
                    vatNumberStatusAttr.Value = vatNumberStatusId.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(vatNumberStatusAttr);
                }
                else
                {
                    vatNumberStatusAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "VatNumberStatus",
                        Value = vatNumberStatusId.ToString()
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(vatNumberStatusAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the street address
        /// </summary>
        public string StreetAddress
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute streetAddressAttr = customerAttributes.FindAttribute("StreetAddress", this.CustomerId);
                if (streetAddressAttr != null)
                    return streetAddressAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute streetAddressAttr = customerAttributes.FindAttribute("StreetAddress", this.CustomerId);
                if (streetAddressAttr != null)
                {
                    streetAddressAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(streetAddressAttr);
                }
                else
                {
                    streetAddressAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "StreetAddress",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(streetAddressAttr);
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the street address 2
        /// </summary>
        public string StreetAddress2
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute streetAddress2Attr = customerAttributes.FindAttribute("StreetAddress2", this.CustomerId);
                if (streetAddress2Attr != null)
                    return streetAddress2Attr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute streetAddress2Attr = customerAttributes.FindAttribute("StreetAddress2", this.CustomerId);
                if (streetAddress2Attr != null)
                {
                    streetAddress2Attr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(streetAddress2Attr);
                }
                else
                {
                    streetAddress2Attr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "StreetAddress2",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(streetAddress2Attr);
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
        public string ZipPostalCode
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute zipPostalCodeAttr = customerAttributes.FindAttribute("ZipPostalCode", this.CustomerId);
                if (zipPostalCodeAttr != null)
                    return zipPostalCodeAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute zipPostalCodeAttr = customerAttributes.FindAttribute("ZipPostalCode", this.CustomerId);
                if (zipPostalCodeAttr != null)
                {
                    zipPostalCodeAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(zipPostalCodeAttr);
                }
                else
                {
                    zipPostalCodeAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "ZipPostalCode",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(zipPostalCodeAttr);
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute cityAttr = customerAttributes.FindAttribute("City", this.CustomerId);
                if (cityAttr != null)
                    return cityAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute cityAttr = customerAttributes.FindAttribute("City", this.CustomerId);
                if (cityAttr != null)
                {
                    cityAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(cityAttr);
                }
                else
                {
                    cityAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "City",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(cityAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute phoneNumberAttr = customerAttributes.FindAttribute("PhoneNumber", this.CustomerId);
                if (phoneNumberAttr != null)
                    return phoneNumberAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute phoneNumberAttr = customerAttributes.FindAttribute("PhoneNumber", this.CustomerId);
                if (phoneNumberAttr != null)
                {
                    phoneNumberAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(phoneNumberAttr);
                }
                else
                {
                    phoneNumberAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "PhoneNumber",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(phoneNumberAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
        public string FaxNumber
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute faxNumberAttr = customerAttributes.FindAttribute("FaxNumber", this.CustomerId);
                if (faxNumberAttr != null)
                    return faxNumberAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                value = value.Trim();

                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute faxNumberAttr = customerAttributes.FindAttribute("FaxNumber", this.CustomerId);
                if (faxNumberAttr != null)
                {
                    faxNumberAttr.Value = value;
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(faxNumberAttr);
                }
                else
                {
                    faxNumberAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "FaxNumber",
                        Value = value
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(faxNumberAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute countryIdAttr = customerAttributes.FindAttribute("CountryId", this.CustomerId);
                if (countryIdAttr != null)
                {
                    int _countryId = 0;
                    int.TryParse(countryIdAttr.Value, out _countryId);
                    return _countryId;
                }
                else
                    return 0;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute countryIdAttr = customerAttributes.FindAttribute("CountryId", this.CustomerId);
                if (countryIdAttr != null)
                {
                    countryIdAttr.Value = value.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(countryIdAttr);
                }
                else
                {
                    countryIdAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "CountryId",
                        Value = value.ToString()
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(countryIdAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public int StateProvinceId
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute stateProvinceIdAttr = customerAttributes.FindAttribute("StateProvinceId", this.CustomerId);
                if (stateProvinceIdAttr != null)
                {
                    int _stateProvinceId = 0;
                    int.TryParse(stateProvinceIdAttr.Value, out _stateProvinceId);
                    return _stateProvinceId;
                }
                else
                    return 0;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute stateProvinceIdAttr = customerAttributes.FindAttribute("StateProvinceId", this.CustomerId);
                if (stateProvinceIdAttr != null)
                {
                    stateProvinceIdAttr.Value = value.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(stateProvinceIdAttr);
                }
                else
                {
                    stateProvinceIdAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "StateProvinceId",
                        Value = value.ToString()
                    };
                   IoC.Resolve<ICustomerService>().InsertCustomerAttribute(stateProvinceIdAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets the value indivating whether customer is agree to receive newsletters
        /// </summary>
        public bool ReceiveNewsletter
        {
            get
            {
                NewsLetterSubscription subscription = this.NewsLetterSubscription;

                return (subscription != null && subscription.Active);
            }
            set
            {
                NewsLetterSubscription subscription = this.NewsLetterSubscription;
                if (subscription != null)
                {
                    if (value)
                    {
                        subscription.Active = true;
                        IoC.Resolve<IMessageService>().UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        IoC.Resolve<IMessageService>().DeleteNewsLetterSubscription(subscription.NewsLetterSubscriptionId);
                    }
                }
                else
                {
                    if (value)
                    {
                        var newsLetterSubscription = new NewsLetterSubscription()
                        {
                            NewsLetterSubscriptionGuid = Guid.NewGuid(),
                            Email = Email,
                            Active = value,
                            CreatedOn = DateTime.UtcNow
                        };
                        IoC.Resolve<IMessageService>().InsertNewsLetterSubscription(newsLetterSubscription);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the NewsLetterSubscription entity if the customer has been subscribed to newsletters
        /// </summary>
        public NewsLetterSubscription NewsLetterSubscription
        {
            get
            {
                return IoC.Resolve<IMessageService>().GetNewsLetterSubscriptionByEmail(Email);
            }
        }

        /// <summary>
        /// Gets or sets the password recovery token
        /// </summary>
        public string PasswordRecoveryToken
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute passwordRecoveryAttr = customerAttributes.FindAttribute("PasswordRecoveryToken", this.CustomerId);
                if (passwordRecoveryAttr != null)
                    return passwordRecoveryAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute passwordRecoveryAttr = customerAttributes.FindAttribute("PasswordRecoveryToken", this.CustomerId);

                if (passwordRecoveryAttr != null)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        passwordRecoveryAttr.Value = value;
                        IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(passwordRecoveryAttr);
                    }
                    else
                    {
                        IoC.Resolve<ICustomerService>().DeleteCustomerAttribute(passwordRecoveryAttr.CustomerAttributeId);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        passwordRecoveryAttr = new CustomerAttribute()
                        {
                            CustomerId = this.CustomerId,
                            Key = "PasswordRecoveryToken",
                            Value = value
                        };
                        IoC.Resolve<ICustomerService>().InsertCustomerAttribute(passwordRecoveryAttr);
                    }
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the account activation token
        /// </summary>
        public string AccountActivationToken
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute accountActivationAttr = customerAttributes.FindAttribute("AccountActivationToken", this.CustomerId);
                if (accountActivationAttr != null)
                    return accountActivationAttr.Value;
                else
                    return string.Empty;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute accountActivationAttr = customerAttributes.FindAttribute("AccountActivationToken", this.CustomerId);

                if (accountActivationAttr != null)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        accountActivationAttr.Value = value;
                        IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(accountActivationAttr);
                    }
                    else
                    {
                        IoC.Resolve<ICustomerService>().DeleteCustomerAttribute(accountActivationAttr.CustomerAttributeId);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        accountActivationAttr = new CustomerAttribute()
                        {
                            CustomerId = this.CustomerId,
                            Key = "AccountActivationToken",
                            Value = value
                        };
                        IoC.Resolve<ICustomerService>().InsertCustomerAttribute(accountActivationAttr);
                    }
                }
                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets reward points collection
        /// </summary>
        public List<RewardPointsHistory> RewardPointsHistory
        {
            get
            {
                if (this.CustomerId == 0)
                    return new List<RewardPointsHistory>();
                if (_rewardPointsHistoryCache == null)
                {
                    _rewardPointsHistoryCache = IoC.Resolve<IOrderService>().GetAllRewardPointsHistoryEntries(this.CustomerId,
                        null, 0, int.MaxValue);
                }
                return _rewardPointsHistoryCache;
            }
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        public int RewardPointsBalance
        {
            get
            {
                int result = 0;
                if (this.RewardPointsHistory.Count > 0)
                    result = this.RewardPointsHistory[0].PointsBalance;
                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use reward points during checkout
        /// </summary>
        public bool UseRewardPointsDuringCheckout
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute useRewardPointsAttr = customerAttributes.FindAttribute("UseRewardPointsDuringCheckout", this.CustomerId);
                if (useRewardPointsAttr != null)
                {
                    bool _useRewardPoints = false;
                    bool.TryParse(useRewardPointsAttr.Value, out _useRewardPoints);
                    return _useRewardPoints;
                }
                else
                    return false;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute useRewardPointsAttr = customerAttributes.FindAttribute("UseRewardPointsDuringCheckout", this.CustomerId);
                if (useRewardPointsAttr != null)
                {
                    useRewardPointsAttr.Value = value.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(useRewardPointsAttr);
                }
                else
                {
                    useRewardPointsAttr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "UseRewardPointsDuringCheckout",
                        Value = value.ToString()
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(useRewardPointsAttr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customer is notified about new private messages
        /// </summary>
        public bool NotifiedAboutNewPrivateMessages
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute attr = customerAttributes.FindAttribute("NotifiedAboutNewPrivateMessages", this.CustomerId);
                if (attr != null)
                {
                    bool _result = false;
                    bool.TryParse(attr.Value, out _result);
                    return _result;
                }
                else
                    return false;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute attr = customerAttributes.FindAttribute("NotifiedAboutNewPrivateMessages", this.CustomerId);
                if (attr != null)
                {
                    attr.Value = value.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(attr);
                }
                else
                {
                    attr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "NotifiedAboutNewPrivateMessages",
                        Value = value.ToString()
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(attr);
                }

                ResetCachedValues();
            }
        }

        /// <summary>
        /// Gets or sets the impersonated customer identifier
        /// </summary>
        public Guid ImpersonatedCustomerGuid
        {
            get
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute attr = customerAttributes.FindAttribute("ImpersonatedCustomerGuid", this.CustomerId);
                if (attr != null)
                {
                    Guid _impersonatedCustomerGuid = Guid.Empty;
                    Guid.TryParse(attr.Value, out _impersonatedCustomerGuid);
                    return _impersonatedCustomerGuid;
                }
                else
                    return Guid.Empty;
            }
            set
            {
                var customerAttributes = this.CustomerAttributes;
                CustomerAttribute attr = customerAttributes.FindAttribute("ImpersonatedCustomerGuid", this.CustomerId);
                if (attr != null)
                {
                    attr.Value = value.ToString();
                    IoC.Resolve<ICustomerService>().UpdateCustomerAttribute(attr);
                }
                else
                {
                    attr = new CustomerAttribute()
                    {
                        CustomerId = this.CustomerId,
                        Key = "ImpersonatedCustomerGuid",
                        Value = value.ToString()
                    };
                    IoC.Resolve<ICustomerService>().InsertCustomerAttribute(attr);
                }
                ResetCachedValues();
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> NpCustomerRoles { get; set; }
        
        /// <summary>
        /// Gets the discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> NpDiscountUsageHistory { get; set; }
        
        /// <summary>
        /// Gets the gift card usage history
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> NpGiftCardUsageHistory { get; set; }
        
        /// <summary>
        /// Gets the reward points usage history
        /// </summary>
        public virtual ICollection<RewardPointsHistory> NpRewardPointsHistory { get; set; }
        
        #endregion
    }
}