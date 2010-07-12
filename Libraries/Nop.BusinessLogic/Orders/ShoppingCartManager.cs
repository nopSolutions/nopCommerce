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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Shopping cart manager
    /// </summary>
    public partial class ShoppingCartManager
    {
        #region Methods

        #region Repository methods

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThan">Older than date and time</param>
        public static void DeleteExpiredShoppingCartItems(DateTime olderThan)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_ShoppingCartItemDeleteExpired(olderThan);
        }

        /// <summary>
        /// Deletes a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">The shopping cart item identifier</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        public static void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData)
        {
            if (resetCheckoutData)
            {
                if (NopContext.Current.Session != null)
                {
                    CustomerManager.ResetCheckoutData(NopContext.Current.Session.CustomerId, false);
                }
            }

            var shoppingCartItem = GetShoppingCartItemById(shoppingCartItemId);
            if (shoppingCartItem == null)
                return;

            if (shoppingCartItem.ShoppingCartType == ShoppingCartTypeEnum.ShoppingCart)
            {
                CustomerActivityManager.InsertActivity(
                    "RemoveFromShoppingCart",
                    LocalizationManager.GetLocaleResourceString("ActivityLog.RemoveFromShoppingCart"),
                    shoppingCartItem.ProductVariant.FullProductName);
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shoppingCartItem))
                context.ShoppingCartItems.Attach(shoppingCartItem);
            context.DeleteObject(shoppingCartItem);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets a shopping cart by customer session GUId
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="customerSessionGuid">The customer session identifier</param>
        /// <returns>Cart</returns>
        public static ShoppingCart GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum shoppingCartType, 
            Guid customerSessionGuid)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sci in context.ShoppingCartItems
                        orderby sci.CreatedOn
                        where sci.ShoppingCartTypeId == (int)shoppingCartType && sci.CustomerSessionGuid == customerSessionGuid
                        select sci;
            var scItems = query.ToList();

            var shoppingCart = new ShoppingCart();
            shoppingCart.AddRange(scItems);
            return shoppingCart;
        }

        /// <summary>
        /// Gets a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">The shopping cart item identifier</param>
        /// <returns>Shopping cart item</returns>
        public static ShoppingCartItem GetShoppingCartItemById(int shoppingCartItemId)
        {
            if (shoppingCartItemId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sci in context.ShoppingCartItems
                        where sci.ShoppingCartItemId == shoppingCartItemId
                        select sci;
            var shoppingCartItem = query.SingleOrDefault();
            return shoppingCartItem;
        }

        /// <summary>
        /// Inserts a shopping cart item
        /// </summary>
        /// <param name="shoppingCartType">The shopping cart type</param>
        /// <param name="customerSessionGuid">The customer session identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="attributesXml">The product variant attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Shopping cart item</returns>
        internal static ShoppingCartItem InsertShoppingCartItem(ShoppingCartTypeEnum shoppingCartType,
            Guid customerSessionGuid, int productVariantId, string attributesXml,
            decimal customerEnteredPrice, int quantity,
            DateTime createdOn, DateTime updatedOn)
        {
            if (attributesXml == null)
                attributesXml = string.Empty;

            var context = ObjectContextHelper.CurrentObjectContext;

            var shoppingCartItem = context.ShoppingCartItems.CreateObject();
            shoppingCartItem.ShoppingCartTypeId = (int)shoppingCartType;
            shoppingCartItem.CustomerSessionGuid = customerSessionGuid;
            shoppingCartItem.ProductVariantId = productVariantId;
            shoppingCartItem.AttributesXml = attributesXml;
            shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.CreatedOn = createdOn;
            shoppingCartItem.UpdatedOn = updatedOn;

            context.ShoppingCartItems.AddObject(shoppingCartItem);
            context.SaveChanges();

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.ShoppingCartType == ShoppingCartTypeEnum.ShoppingCart)
                {
                    CustomerActivityManager.InsertActivity(
                        "AddToShoppingCart",
                        LocalizationManager.GetLocaleResourceString("ActivityLog.AddToShoppingCart"),
                        shoppingCartItem.ProductVariant.FullProductName);
                }
            }

            return shoppingCartItem;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">The shopping cart item identifier</param>
        /// <param name="shoppingCartType">The shopping cart type</param>
        /// <param name="customerSessionGuid">The customer session identifier</param>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <param name="attributesXml">The product variant attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">The quantity</param>
        /// <param name="createdOn">The date and time of instance creation</param>
        /// <param name="updatedOn">The date and time of instance update</param>
        /// <returns>Shopping cart item</returns>
        internal static ShoppingCartItem UpdateShoppingCartItem(int shoppingCartItemId,
            ShoppingCartTypeEnum shoppingCartType, Guid customerSessionGuid,
            int productVariantId, string attributesXml, decimal customerEnteredPrice,
            int quantity, DateTime createdOn, DateTime updatedOn)
        {
            if (shoppingCartItemId == 0)
                return null;

            if (attributesXml == null)
                attributesXml = string.Empty;

            var shoppingCartItem = GetShoppingCartItemById(shoppingCartItemId);
            if (shoppingCartItem == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(shoppingCartItem))
                context.ShoppingCartItems.Attach(shoppingCartItem);

            shoppingCartItem.ShoppingCartTypeId = (int)shoppingCartType;
            shoppingCartItem.CustomerSessionGuid = customerSessionGuid;
            shoppingCartItem.ProductVariantId = productVariantId;
            shoppingCartItem.AttributesXml = attributesXml;
            shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
            shoppingCartItem.Quantity = quantity;
            shoppingCartItem.CreatedOn = createdOn;
            shoppingCartItem.UpdatedOn = updatedOn;
            context.SaveChanges();

            return shoppingCartItem;
        }

        /// <summary>
        /// Gets current user shopping cart
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <returns>Cart</returns>
        public static ShoppingCart GetCurrentShoppingCart(ShoppingCartTypeEnum shoppingCartType)
        {
            if (NopContext.Current.Session == null)
                return new ShoppingCart();
            var customerSessionGuid = NopContext.Current.Session.CustomerSessionGuid;
            return GetShoppingCartByCustomerSessionGuid(shoppingCartType, customerSessionGuid);
        }

        /// <summary>
        /// Gets shopping cart
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <returns>Cart</returns>
        public static ShoppingCart GetCustomerShoppingCart(int customerId, 
            ShoppingCartTypeEnum shoppingCartType)
        {
            var customerSession = CustomerManager.GetCustomerSessionByCustomerId(customerId);
            if (customerSession == null)
                return new ShoppingCart();
            var customerSessionGuid = customerSession.CustomerSessionGuid;
            return GetShoppingCartByCustomerSessionGuid(shoppingCartType, customerSessionGuid);
        }
        
        #endregion

        #region Workflow/helper methods

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public static decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer)
        {
            bool useRewardPoints = false;
            if (customer != null)
                useRewardPoints = customer.UseRewardPointsDuringCheckout;
            return GetShoppingCartTotal(cart, paymentMethodId, customer, useRewardPoints);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public static decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer, bool useRewardPoints)
        {
            int redeemedRewardPoints = 0;
            decimal redeemedRewardPointsAmount = decimal.Zero;
            List<AppliedGiftCard> appliedGiftCards = null;
            return GetShoppingCartTotal(cart, paymentMethodId, customer,
                out appliedGiftCards, useRewardPoints, 
                out redeemedRewardPoints, out redeemedRewardPointsAmount);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public static decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer,
            out List<AppliedGiftCard> appliedGiftCards, bool useRewardPoints,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount)
        {
            string subTotalError = string.Empty;
            string shippingError = string.Empty;
            string paymentMethodAdditionalFeeError = string.Empty;
            string taxError = string.Empty;
            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            //subtotal without tax
            decimal subTotalDiscountBase = decimal.Zero;
            Discount appliedDiscount = null;
            decimal subtotalBaseWithoutPromo = decimal.Zero;
            decimal subtotalBaseWithPromo = decimal.Zero;
            subTotalError = ShoppingCartManager.GetShoppingCartSubTotal(cart,
                customer, out subTotalDiscountBase,
                out appliedDiscount, false,
                out subtotalBaseWithoutPromo, out subtotalBaseWithPromo);

            //shipping without tax
            decimal? shoppingCartShipping = ShippingManager.GetShoppingCartShippingTotal(cart, customer, false, ref shippingError);

            //payment method additional fee without tax
            decimal paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (paymentMethodId > 0)
            {
                decimal paymentMethodAdditionalFee = PaymentManager.GetAdditionalHandlingFee(paymentMethodId);
                paymentMethodAdditionalFeeWithoutTax = TaxManager.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
                    false, customer, ref paymentMethodAdditionalFeeError);
            }

            //tax
            decimal shoppingCartTax = TaxManager.GetTaxTotal(cart, paymentMethodId, customer, ref taxError);

            //order total
            decimal resultTemp = decimal.Zero;
            resultTemp += subtotalBaseWithPromo;
            if (shoppingCartShipping.HasValue)
            {
                resultTemp += shoppingCartShipping.Value;
            }
            resultTemp += paymentMethodAdditionalFeeWithoutTax;
            resultTemp += shoppingCartTax;
            resultTemp = Math.Round(resultTemp, 2);

            #region Gift Cards

            //let's apply gift cards now (gift cards that can be used)
            appliedGiftCards = new List<AppliedGiftCard>();
            if (!cart.IsRecurring)
            {
                //we don't apply gift cards for recurring products
                var giftCards = GiftCardHelper.GetActiveGiftCards(customer);
                foreach (var gc in giftCards)
                {
                    if (resultTemp > decimal.Zero)
                    {
                        decimal remainingAmount = GiftCardHelper.GetGiftCardRemainingAmount(gc);
                        decimal amountCanBeUsed = decimal.Zero;
                        if (resultTemp > remainingAmount)
                            amountCanBeUsed = remainingAmount;
                        else
                            amountCanBeUsed = resultTemp;

                        //reduce subtotal
                        resultTemp -= amountCanBeUsed;

                        AppliedGiftCard appliedGiftCard = new AppliedGiftCard();
                        appliedGiftCard.GiftCardId = gc.GiftCardId;
                        appliedGiftCard.AmountCanBeUsed = amountCanBeUsed;
                        appliedGiftCards.Add(appliedGiftCard);
                    }
                }
            }

            #endregion

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            resultTemp = Math.Round(resultTemp, 2);

            decimal? orderTotal = null;
            if (!String.IsNullOrEmpty(subTotalError) ||
                !String.IsNullOrEmpty(shippingError) ||
                !String.IsNullOrEmpty(paymentMethodAdditionalFeeError) ||
                !String.IsNullOrEmpty(taxError))
            {
                //return null if we have errors
                orderTotal = null;
                return orderTotal;
            }
            else
            {
                //return result if we have no errors
                orderTotal = resultTemp;
            }

            #region Reward points
            if (OrderManager.RewardPointsEnabled && useRewardPoints && customer != null)
            {
                int rewardPointsBalance = customer.RewardPointsBalance;
                decimal rewardPointsBalanceAmount = OrderManager.ConvertRewardPointsToAmount(rewardPointsBalance);
                if (orderTotal.HasValue && orderTotal.Value > decimal.Zero)
                {
                    if (orderTotal.Value > rewardPointsBalanceAmount)
                    {
                        redeemedRewardPoints = rewardPointsBalance;
                        redeemedRewardPointsAmount = rewardPointsBalanceAmount;
                    }
                    else
                    {
                        redeemedRewardPointsAmount = orderTotal.Value;
                        redeemedRewardPoints = OrderManager.ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
                    }
                }
            }
            #endregion

            if (orderTotal.HasValue)
            {
                orderTotal = orderTotal.Value - redeemedRewardPointsAmount;
                orderTotal = Math.Round(orderTotal.Value, 2);
                return orderTotal;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="discountAmount">Subtotal discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subtotalWithoutPromo">Sub total without promo (discounts, gift cards)</param>
        /// <param name="subtotalWithPromo">Sub total with promo (discounts, gift cards)</param>
        /// <returns>Error</returns>
        public static string GetShoppingCartSubTotal(ShoppingCart cart, Customer customer,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subtotalWithoutPromo, out decimal subtotalWithPromo)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShoppingCartSubTotal(cart, customer, out discountAmount,
                out appliedDiscount, includingTax,
                out subtotalWithoutPromo, out subtotalWithPromo);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="discountAmount">Subtotal discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="subtotalWithoutPromo">Sub total without promo (discounts, gift cards)</param>
        /// <param name="subtotalWithPromo">Sub total with promo (discounts, gift cards)</param>
        /// <returns>Error</returns>
        public static string GetShoppingCartSubTotal(ShoppingCart cart, Customer customer,
            out decimal discountAmount, out Discount appliedDiscount, bool includingTax,
            out decimal subtotalWithoutPromo, out decimal subtotalWithPromo)
        {
            string error = string.Empty;

            //sub totals without discount and gift cards
            decimal subTotalExclTaxWithoutDiscount = decimal.Zero;
            decimal subTotalInclTaxWithoutDiscount = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                string error2 = string.Empty;
                decimal sciSubTotal = PriceHelper.GetSubTotal(shoppingCartItem, customer, true);
                subTotalExclTaxWithoutDiscount += TaxManager.GetPrice(shoppingCartItem.ProductVariant, sciSubTotal, false, customer, ref error2);
                if (!String.IsNullOrEmpty(error2))
                {
                    error = error2;
                }
                subTotalInclTaxWithoutDiscount += TaxManager.GetPrice(shoppingCartItem.ProductVariant, sciSubTotal, true, customer, ref error2);
                if (!String.IsNullOrEmpty(error2))
                {
                    error = error2;
                }
            }

            //checkout attributes
            if (customer != null)
            {
                var caValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(customer.CheckoutAttributes);
                foreach (var caValue in caValues)
                {
                    string error3 = string.Empty;
                    subTotalExclTaxWithoutDiscount += TaxManager.GetCheckoutAttributePrice(caValue, false, customer, ref error3);
                    if (!String.IsNullOrEmpty(error3))
                    {
                        error = error3;
                    }
                    subTotalInclTaxWithoutDiscount += TaxManager.GetCheckoutAttributePrice(caValue, true, customer, ref error3);
                    if (!String.IsNullOrEmpty(error3))
                    {
                        error = error3;
                    }
                }
            }

            if (includingTax)
                subtotalWithoutPromo = subTotalInclTaxWithoutDiscount;
            else
                subtotalWithoutPromo = subTotalExclTaxWithoutDiscount;
            

            #region Discounts
            //Discount amount (excl tax)
            //We calculate discount amount on subtotal excl tax
            //This type of discounts [Assigned to whole order] is not taxable
            discountAmount = GetOrderDiscount(customer, subTotalExclTaxWithoutDiscount, out appliedDiscount);

            //sub totals with discount
            decimal subTotalWithDiscount = decimal.Zero;
            if (includingTax)
            {
                if (subTotalInclTaxWithoutDiscount < discountAmount)
                    discountAmount = subTotalInclTaxWithoutDiscount;

                subTotalWithDiscount = subTotalInclTaxWithoutDiscount - discountAmount;
            }
            else
            {
                if (subTotalExclTaxWithoutDiscount < discountAmount)
                    discountAmount = subTotalExclTaxWithoutDiscount;

                subTotalWithDiscount = subTotalExclTaxWithoutDiscount - discountAmount;
            }

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;
            subTotalWithDiscount = Math.Round(subTotalWithDiscount, 2);

            #endregion

            subtotalWithPromo = subTotalWithDiscount;
            if (subtotalWithPromo < decimal.Zero)
                subtotalWithPromo = decimal.Zero;
            subtotalWithPromo = Math.Round(subtotalWithPromo, 2);

            return error;
        }

        /// <summary>
        /// Gets an order discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Order discount</returns>
        public static decimal GetOrderDiscount(Customer customer, 
            decimal orderSubTotal, out Discount appliedDiscount)
        {
            decimal subTotalDiscountAmount = decimal.Zero;

            string customerCouponCode = string.Empty;
            if (customer != null)
                customerCouponCode = customer.LastAppliedCouponCode;

            var allDiscounts = DiscountManager.GetAllDiscounts(DiscountTypeEnum.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<Discount>();
            foreach (var _discount in allDiscounts)
            {
                if (_discount.IsActive(customerCouponCode) &&
                    _discount.DiscountType == DiscountTypeEnum.AssignedToOrderSubTotal &&
                    !allowedDiscounts.ContainsDiscount(_discount.Name))
                {
                    //discount requirements
                    if (_discount.CheckDiscountRequirements(customer)
                        && _discount.CheckDiscountLimitations(customer))
                    {
                        allowedDiscounts.Add(_discount);
                    }
                }
            }

            appliedDiscount = DiscountManager.GetPreferredDiscount(allowedDiscounts, orderSubTotal);
            if (appliedDiscount != null)
            {
                subTotalDiscountAmount = appliedDiscount.GetDiscountAmount(orderSubTotal);
            }

            if (subTotalDiscountAmount < decimal.Zero)
                subTotalDiscountAmount = decimal.Zero;

            subTotalDiscountAmount = Math.Round(subTotalDiscountAmount, 2);

            return subTotalDiscountAmount;
        }
              
        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public static List<string> GetShoppingCartItemWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, decimal customerEnteredPrice, 
            int quantity)
        {
            var warnings = new List<string>();
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                warnings.Add(string.Format("Product variant (Id={0}) can not be loaded", productVariantId));
                return warnings;
            }

            var product = productVariant.Product;
            if (product == null)
            {
                warnings.Add(string.Format("Product (Id={0}) can not be loaded", productVariant.ProductId));
                return warnings;
            }

            if (product.Deleted || productVariant.Deleted)
            {
                warnings.Add("Product is deleted");
                return warnings;
            }

            if (!product.Published || !productVariant.Published)
            {
                warnings.Add("Product is not published");
            }

            if (productVariant.DisableBuyButton)
            {
                warnings.Add("Buying is disabled");
            }

            if (productVariant.CustomerEntersPrice)
            {
                if (customerEnteredPrice < productVariant.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > productVariant.MaximumCustomerEnteredPrice)
                {
                    int minimumCustomerEnteredPrice = Convert.ToInt32(Math.Ceiling(CurrencyManager.ConvertCurrency(productVariant.MinimumCustomerEnteredPrice, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency)));
                    int maximumCustomerEnteredPrice = Convert.ToInt32(Math.Truncate(CurrencyManager.ConvertCurrency(productVariant.MaximumCustomerEnteredPrice, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency)));
                
                    warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        minimumCustomerEnteredPrice, maximumCustomerEnteredPrice));
                }
            }

            if (quantity < productVariant.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.MinimumQuantity"), productVariant.OrderMinimumQuantity));
            }

            if (quantity > productVariant.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.MaximumQuantity"), productVariant.OrderMaximumQuantity));
            }

            switch ((ManageInventoryMethodEnum)productVariant.ManageInventory)
            {
                case ManageInventoryMethodEnum.DontManageStock:
                    {
                    }
                    break;
                case ManageInventoryMethodEnum.ManageStock:
                    {
                        if ((BackordersModeEnum)productVariant.Backorders == BackordersModeEnum.NoBackorders)
                        {
                            if (productVariant.StockQuantity < quantity)
                            {
                                int maximumQuantityCanBeAdded = productVariant.StockQuantity;
                                if (maximumQuantityCanBeAdded <= 0)
                                    warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCart.OutOfStock"));
                                else
                                    warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                            }
                        }
                    }
                    break;
                case ManageInventoryMethodEnum.ManageStockByAttributes:
                    {
                        var combination = ProductAttributeManager.FindProductVariantAttributeCombination(productVariant.ProductVariantId, selectedAttributes);
                        if (combination != null)
                        {
                            if (!combination.AllowOutOfStockOrders)
                            {
                                if (combination.StockQuantity < quantity)
                                {
                                    int maximumQuantityCanBeAdded = combination.StockQuantity;
                                    if (maximumQuantityCanBeAdded <= 0)
                                        warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCart.OutOfStock"));
                                    else
                                        warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            if (productVariant.AvailableStartDateTime.HasValue)
            {
                DateTime _now = DateTime.UtcNow;
                DateTime _availableStartDateTime = DateTime.SpecifyKind(productVariant.AvailableStartDateTime.Value, DateTimeKind.Utc);
                if (_availableStartDateTime.CompareTo(_now) > 0)
                {
                    warnings.Add("Product is not available");
                }
            }
            else if (productVariant.AvailableEndDateTime.HasValue)
            {
                DateTime _now = DateTime.UtcNow;
                DateTime _availableEndDateTime = DateTime.SpecifyKind(productVariant.AvailableEndDateTime.Value, DateTimeKind.Utc);
                if (_availableEndDateTime.CompareTo(_now) < 0)
                {
                    warnings.Add("Product is not available");
                }
            }

            //selected attributes
            warnings.AddRange(GetShoppingCartItemAttributeWarnings(shoppingCartType, productVariantId, selectedAttributes, quantity));

            //gift cards
            warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, productVariantId, selectedAttributes));

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public static List<string> GetShoppingCartItemAttributeWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, int quantity)
        {
            return GetShoppingCartItemAttributeWarnings(shoppingCartType,
                productVariantId, selectedAttributes, quantity, true);
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="validateQuantity">Value indicating whether to validation quantity</param>
        /// <returns>Warnings</returns>
        public static List<string> GetShoppingCartItemAttributeWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, int quantity, bool validateQuantity)
        {
            var warnings = new List<string>();
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                warnings.Add(string.Format("Product variant (Id={0}) can not be loaded", productVariantId));
                return warnings;
            }

            //selected attributes
            var pva1Collection = ProductAttributeHelper.ParseProductVariantAttributes(selectedAttributes);
            foreach (var pva1 in pva1Collection)
            {
                var pv1 = pva1.ProductVariant;
                if (pv1 != null)
                {
                    if (pv1.ProductVariantId != productVariant.ProductVariantId)
                    {
                        warnings.Add("Attribute error");
                    }
                }
                else
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }
            }

            //existing product attributes
            var pva2Collection = productVariant.ProductVariantAttributes;
            foreach (var pva2 in pva2Collection)
            {
                if (pva2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var pva1 in pva1Collection)
                    {
                        if (pva1.ProductVariantAttributeId == pva2.ProductVariantAttributeId)
                        {
                            var pvaValuesStr = ProductAttributeHelper.ParseValues(selectedAttributes, pva1.ProductVariantAttributeId);
                            foreach (string str1 in pvaValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        if (!string.IsNullOrEmpty(pva2.TextPrompt))
                        {
                            warnings.Add(pva2.TextPrompt);
                        }
                        else
                        {
                            warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.SelectAttribute"), pva2.ProductAttribute.LocalizedName));
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public static List<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes)
        {
            var warnings = new List<string>();
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                warnings.Add(string.Format("Product variant (Id={0}) can not be loaded", productVariantId));
                return warnings;
            }

            //gift cards
            if (productVariant.IsGiftCard)
            {
                string giftCardRecipientName = string.Empty;
                string giftCardRecipientEmail = string.Empty;
                string giftCardSenderName = string.Empty;
                string giftCardSenderEmail = string.Empty;
                string giftCardMessage = string.Empty;
                ProductAttributeHelper.GetGiftCardAttribute(selectedAttributes,
                    out giftCardRecipientName, out giftCardRecipientEmail,
                    out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                if (String.IsNullOrEmpty(giftCardRecipientName))
                    warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCartWarning.RecipientNameError"));

                if (String.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                    warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCartWarning.RecipientEmailError"));

                if (String.IsNullOrEmpty(giftCardSenderName))
                    warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCartWarning.SenderNameError"));

                if (String.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                    warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCartWarning.SenderEmailError"));
            }

            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public static List<string> GetShoppingCartWarnings(ShoppingCart shoppingCart, string checkoutAttributes, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;
            int cycleLength = 0;
            int cyclePeriod = 0;
            int totalCycles = 0;

            foreach (var sci in shoppingCart)
            {
                var productVariant = sci.ProductVariant;
                if (productVariant == null)
                {
                    warnings.Add(string.Format("Product variant (Id={0}) can not be loaded", sci.ProductVariantId));
                    return warnings;
                }

                if (productVariant.IsRecurring)
                {
                    hasRecurringProducts = true;
                }
                else
                {
                    hasStandartProducts = true;
                }
            }

            if (hasStandartProducts && hasRecurringProducts)
            {
                warnings.Add(LocalizationManager.GetLocaleResourceString("ShoppingCart.CannotMixStandardAndAutoshipProducts"));
            }

            if (hasRecurringProducts)
            {
                string cyclesError = GetReccuringCycleInfo(shoppingCart, out cycleLength, out cyclePeriod, out totalCycles);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (validateCheckoutAttributes)
            {
                //selected attributes
                var ca1Collection = CheckoutAttributeHelper.ParseCheckoutAttributes(checkoutAttributes);

                //existing checkout attributes
                bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(shoppingCart);
                var ca2Collection = CheckoutAttributeManager.GetAllCheckoutAttributes(!shoppingCartRequiresShipping);
                foreach (var ca2 in ca2Collection)
                {
                    if (ca2.IsRequired)
                    {
                        bool found = false;
                        //selected checkout attributes
                        foreach (var ca1 in ca1Collection)
                        {
                            if (ca1.CheckoutAttributeId == ca2.CheckoutAttributeId)
                            {
                                var caValuesStr = CheckoutAttributeHelper.ParseValues(checkoutAttributes, ca1.CheckoutAttributeId);
                                foreach (string str1 in caValuesStr)
                                {
                                    if (!String.IsNullOrEmpty(str1.Trim()))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }

                        //if not found
                        if (!found)
                        {
                            if (!string.IsNullOrEmpty(ca2.LocalizedTextPrompt))
                            {
                                warnings.Add(ca2.LocalizedTextPrompt);
                            }
                            else
                            {
                                warnings.Add(string.Format(LocalizationManager.GetLocaleResourceString("ShoppingCart.SelectAttribute"), ca2.LocalizedName));
                            }
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="cycleLength">Cycle length</param>
        /// <param name="cyclePeriod">Cycle period</param>
        /// <param name="totalCycles">Toital cycles</param>
        /// <returns>Error</returns>
        public static string GetReccuringCycleInfo(ShoppingCart shoppingCart, 
            out int cycleLength, out int cyclePeriod, out int totalCycles)
        {
            string error = string.Empty;

            cycleLength = 0;
            cyclePeriod = 0;
            totalCycles = 0;

            int? _cycleLength = null;
            int? _cyclePeriod = null;
            int? _totalCycles = null;

            foreach (var sci in shoppingCart)
            {
                var productVariant = sci.ProductVariant;
                if (productVariant == null)
                {
                    throw new NopException(string.Format("Product variant (Id={0}) can not be loaded", sci.ProductVariantId));
                }

                if (productVariant.IsRecurring)
                {
                    //cycle length
                    if (_cycleLength.HasValue && _cycleLength.Value != productVariant.CycleLength)
                    {
                        error = LocalizationManager.GetLocaleResourceString("ShoppingCart.ConflictingShipmentSchedules");
                        return error;
                    }
                    else
                    {
                        _cycleLength = productVariant.CycleLength;
                    }

                    //cycle period
                    if (_cyclePeriod.HasValue && _cyclePeriod.Value != productVariant.CyclePeriod)
                    {
                        error = LocalizationManager.GetLocaleResourceString("ShoppingCart.ConflictingShipmentSchedules");
                        return error;
                    }
                    else
                    {
                        _cyclePeriod = productVariant.CyclePeriod;
                    }

                    //total cycles
                    if (_totalCycles.HasValue && _totalCycles.Value != productVariant.TotalCycles)
                    {
                        error = LocalizationManager.GetLocaleResourceString("ShoppingCart.ConflictingShipmentSchedules");
                        return error;
                    }
                    else
                    {
                        _totalCycles = productVariant.TotalCycles;
                    }
                }
            }

            if (!_cycleLength.HasValue || !_cyclePeriod.HasValue || !_totalCycles.HasValue)
            {
                error = "No recurring products";
            }
            else
            {
                cycleLength = _cycleLength.Value;
                cyclePeriod = _cyclePeriod.Value;
                totalCycles = _totalCycles.Value;
            }

            return error;
        }

        /// <summary>
        /// Add a product variant to shopping cart
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public static List<string> AddToCart(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, 
            decimal customerEnteredPrice, int quantity)
        {
            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartTypeEnum.Wishlist && !SettingManager.GetSettingValueBoolean("Common.EnableWishlist"))
                return warnings;

            if (NopContext.Current.Session == null)
                NopContext.Current.Session = NopContext.Current.GetSession(true);

            var customerSessionGuid = NopContext.Current.Session.CustomerSessionGuid;

            CustomerManager.ResetCheckoutData(NopContext.Current.Session.CustomerId, false);

            var cart = GetShoppingCartByCustomerSessionGuid(shoppingCartType, customerSessionGuid);
            ShoppingCartItem shoppingCartItem = null;
            
            foreach (var _shoppingCartItem in cart)
            {
                if (_shoppingCartItem.ProductVariantId == productVariantId)
                {
                    //attributes
                    bool attributesEqual = ProductAttributeHelper.AreProductAttributesEqual(_shoppingCartItem.AttributesXml, selectedAttributes);

                    //gift cards
                    bool giftCardInfoSame = true;
                    if (_shoppingCartItem.ProductVariant.IsGiftCard)
                    {
                        string giftCardRecipientName1 = string.Empty;
                        string giftCardRecipientEmail1 = string.Empty;
                        string giftCardSenderName1 = string.Empty;
                        string giftCardSenderEmail1 = string.Empty;
                        string giftCardMessage1 = string.Empty;
                        ProductAttributeHelper.GetGiftCardAttribute(selectedAttributes,
                            out giftCardRecipientName1, out giftCardRecipientEmail1,
                            out giftCardSenderName1, out giftCardSenderEmail1, out giftCardMessage1);

                        string giftCardRecipientName2 = string.Empty;
                        string giftCardRecipientEmail2 = string.Empty;
                        string giftCardSenderName2 = string.Empty;
                        string giftCardSenderEmail2 = string.Empty;
                        string giftCardMessage2 = string.Empty;
                        ProductAttributeHelper.GetGiftCardAttribute(_shoppingCartItem.AttributesXml,
                            out giftCardRecipientName2, out giftCardRecipientEmail2,
                            out giftCardSenderName2, out giftCardSenderEmail2, out giftCardMessage2);


                        if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                            giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                            giftCardInfoSame = false;
                    }

                    //price is the same (for products which requires customers to enter a price)
                    bool customerEnteredPricesEqual = true;
                    if (_shoppingCartItem.ProductVariant.CustomerEntersPrice)
                    {
                        customerEnteredPricesEqual = Math.Round(_shoppingCartItem.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);
                    }

                    if (attributesEqual &&
                        giftCardInfoSame &&
                        customerEnteredPricesEqual)
                        shoppingCartItem = _shoppingCartItem;
                }
            }

            DateTime now = DateTime.UtcNow;
            if (shoppingCartItem != null)
            {
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(shoppingCartType, productVariantId, 
                    selectedAttributes, customerEnteredPrice, newQuantity));

                if (warnings.Count == 0)
                {
                    UpdateShoppingCartItem(shoppingCartItem.ShoppingCartItemId, 
                        shoppingCartType,
                        customerSessionGuid, 
                        productVariantId, 
                        selectedAttributes,
                        shoppingCartItem.CustomerEnteredPrice,
                        newQuantity,
                        shoppingCartItem.CreatedOn, 
                        now);
                }
            }
            else
            {
                warnings.AddRange(GetShoppingCartItemWarnings(shoppingCartType, productVariantId, 
                    selectedAttributes, customerEnteredPrice, quantity));
                if (warnings.Count == 0)
                {
                    //maximum items validation
                    if (shoppingCartType == ShoppingCartTypeEnum.ShoppingCart)
                    {
                        if (cart.Count >= SettingManager.GetSettingValueInteger("Common.MaximumShoppingCartItems", 1000))
                            return warnings;
                    }
                    else if (shoppingCartType == ShoppingCartTypeEnum.Wishlist)
                    {
                        if (cart.Count >= SettingManager.GetSettingValueInteger("Common.MaximumWishlistItems", 1000))
                            return warnings;
                    }

                    //insert item
                    InsertShoppingCartItem(shoppingCartType,
                        customerSessionGuid, 
                        productVariantId, 
                        selectedAttributes,
                        customerEnteredPrice,
                        quantity, 
                        now, 
                        now);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public static List<string> UpdateCart(int shoppingCartItemId, int newQuantity,
            bool resetCheckoutData)
        {
            var warnings = new List<string>();

            if (NopContext.Current.Session == null)
                return warnings;

            var shoppingCartItem = GetShoppingCartItemById(shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    CustomerManager.ResetCheckoutData(NopContext.Current.Session.CustomerId, false);
                }
                if (newQuantity > 0)
                {
                    warnings.AddRange(GetShoppingCartItemWarnings(shoppingCartItem.ShoppingCartType,
                        shoppingCartItem.ProductVariantId, shoppingCartItem.AttributesXml, 
                        shoppingCartItem.CustomerEnteredPrice, newQuantity));
                    if (warnings.Count == 0)
                    {
                        UpdateShoppingCartItem(
                            shoppingCartItem.ShoppingCartItemId, 
                            shoppingCartItem.ShoppingCartType, 
                            shoppingCartItem.CustomerSessionGuid,
                            shoppingCartItem.ProductVariantId, 
                            shoppingCartItem.AttributesXml,
                            shoppingCartItem.CustomerEnteredPrice,
                            newQuantity, 
                            shoppingCartItem.CreatedOn,
                            DateTime.UtcNow);
                    }
                }
                else
                {
                    DeleteShoppingCartItem(shoppingCartItem.ShoppingCartItemId, resetCheckoutData);
                }
            }

            return warnings;
        }

        #endregion

        #endregion
    }
}
