
using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Discounts;
using System.Diagnostics;
using System.Xml;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        public Customer()
        {
            this.CustomerContent = new List<CustomerContent>();
            this.CustomerRoles = new List<CustomerRole>();
            this.CustomerAttributes = new List<CustomerAttribute>();
            this.Addresses = new List<Address>();
            this.ShoppingCartItems = new List<ShoppingCartItem>();
            this.Orders = new List<Order>();
            this.RewardPointsHistory = new List<RewardPointsHistory>();
            this.ReturnRequests = new List<ReturnRequest>();
        }

        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the associated user identifier
        /// </summary>
        public int? AssociatedUserId { get; set; }

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
        /// Gets or sets the tax display type identifier
        /// </summary>
        public int TaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a VAT number (including counry code)
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number status identifier
        /// </summary>
        public int VatNumberStatusId { get; set; }

        /// <summary>
        /// Gets or sets the last payment method system name (selected one)
        /// </summary>
        public string SelectedPaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the selected checkout attributes (serialized)
        /// </summary>
        public string CheckoutAttributes { get; set; }

        /// <summary>
        /// Gets or sets the applied discount coupon code
        /// </summary>
        public string DiscountCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the applied gift card coupon codes (serialized)
        /// </summary>
        public string GiftCardCouponCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use reward points during checkout
        /// </summary>
        public bool UseRewardPointsDuringCheckout { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }
        
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
        
        #region Custom properties

        /// <summary>
        /// Gets the tax display type
        /// </summary>
        public TaxDisplayType TaxDisplayType
        {
            get
            {
                return (TaxDisplayType)this.TaxDisplayTypeId;
            }
            set
            {
                this.TaxDisplayTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the VAT number status
        /// </summary>
        public VatNumberStatus VatNumberStatus
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
        /// Gets or sets shopping cart items
        /// </summary>
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets orders
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets reward points history
        /// </summary>
        public virtual ICollection<RewardPointsHistory> RewardPointsHistory { get; set; }

        /// <summary>
        /// Gets or sets return request of this customer
        /// </summary>
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
        
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

        /// <summary>
        /// Gets the activity log
        /// </summary>
        public virtual ICollection<ActivityLog> ActivityLog { get; set; }

        #endregion

        #region Addresses

        public virtual void AddAddress(Address address)
        {
            if (!this.Addresses.Contains(address))
                this.Addresses.Add(address);
        }

        public virtual void RemoveAddress(Address address)
        {
            if (this.Addresses.Contains(address))
            {
                if (this.BillingAddress == address) this.BillingAddress = null;
                if (this.ShippingAddress == address) this.ShippingAddress = null;

                this.Addresses.Remove(address);
            }
        }

        public virtual void SetBillingAddress(Address address)
        {
            if (this.Addresses.Contains(address))
                this.BillingAddress = address;
        }

        public virtual void SetShippingAddress(Address address)
        {
            if (this.Addresses.Contains(address))
                this.ShippingAddress = address;
        }

        #endregion

        #region Reward points

        public virtual void AddRewardPointsHistoryEntry(int points, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M)
        {
            int newPointsBalance = this.GetRewardPointsBalance() + points;

            var rewardPointsHistory = new RewardPointsHistory()
            {
                Customer = this,
                UsedWithOrder = usedWithOrder,
                Points = points,
                PointsBalance = newPointsBalance,
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };

            this.RewardPointsHistory.Add(rewardPointsHistory);
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        public virtual int GetRewardPointsBalance()
        {
            int result = 0;
            if (this.RewardPointsHistory.Count > 0)
                result = this.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id).FirstOrDefault().PointsBalance;
            return result;
        }

        #endregion

        #region Gift cards

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <returns>Coupon codes</returns>
        public string[] ParseAppliedGiftCardCouponCodes()
        {
            string serializedGiftCartCouponCodes = this.GiftCardCouponCodes;

            var couponCodes = new List<string>();
            if (String.IsNullOrEmpty(serializedGiftCartCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(serializedGiftCartCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Code"] != null)
                    {
                        string code = node1.Attributes["Code"].InnerText.Trim();
                        couponCodes.Add(code);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        public void ApplyGiftCardCouponCode(string couponCode)
        {
            string result = string.Empty;
            try
            {
                var serializedGiftCartCouponCodes = this.GiftCardCouponCodes;

                couponCode = couponCode.Trim().ToLower();

                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(serializedGiftCartCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(serializedGiftCartCouponCodes);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Code"] != null)
                    {
                        string _couponCode = node1.Attributes["Code"].InnerText.Trim();
                        if (_couponCode.ToLower() == couponCode.ToLower())
                        {
                            gcElement = (XmlElement)node1;
                            break;
                        }
                    }
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            //apply new value
            this.GiftCardCouponCodes = result;
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        public void RemoveGiftCardCouponCode(string couponCode)
        {
            //get applied coupon codes
            var existingCouponCodes = ParseAppliedGiftCardCouponCodes();

            //clear them
            this.GiftCardCouponCodes = string.Empty;

            //save again except removed one
            foreach (string existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    ApplyGiftCardCouponCode(existingCouponCode);
        }
        
        #endregion
    }
}