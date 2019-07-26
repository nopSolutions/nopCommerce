using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderTotalCalculationService : IOrderTotalCalculationService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IDiscountService _discountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OrderTotalCalculationService(CatalogSettings catalogSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            IDiscountService discountService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IRewardPointService rewardPointService,
            IShippingPluginManager shippingPluginManager,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings)
        {
            _catalogSettings = catalogSettings;
            _checkoutAttributeParser = checkoutAttributeParser;
            _discountService = discountService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _rewardPointService = rewardPointService;
            _shippingPluginManager = shippingPluginManager;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _taxService = taxService;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderSubtotalDiscount(Customer customer,
            decimal orderSubTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
            {
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        _discountService.ValidateDiscount(discount, customer).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }
            }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderSubTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return discountAmount;
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping discount</returns>
        protected virtual decimal GetShippingDiscount(Customer customer, decimal shippingTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return shippingDiscountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        _discountService.ValidateDiscount(discount, customer).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = _priceCalculationService.RoundPrice(shippingDiscountAmount);

            return shippingDiscountAmount;
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Order discount</returns>
        protected virtual decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return discountAmount;

            var allDiscounts = _discountService.GetAllDiscountsForCaching(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<DiscountForCaching>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        _discountService.ValidateDiscount(discount, customer).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = _priceCalculationService.RoundPrice(discountAmount);

            return discountAmount;
        }

        /// <summary>
        /// Update order total
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="subTotalExclTax">Subtotal (excl tax)</param>
        /// <param name="discountAmountExclTax">Discount amount (excl tax)</param>
        /// <param name="shippingTotalExclTax">Shipping (excl tax)</param>
        /// <param name="updatedOrder">Order</param>
        /// <param name="taxTotal">Tax</param>
        /// <param name="customer">Customer</param>
        protected virtual void UpdateTotal(UpdateOrderParameters updateOrderParameters, decimal subTotalExclTax,
            decimal discountAmountExclTax, decimal shippingTotalExclTax, Order updatedOrder, decimal taxTotal, Customer customer)
        {
            var total = subTotalExclTax - discountAmountExclTax + shippingTotalExclTax + updatedOrder.PaymentMethodAdditionalFeeExclTax + taxTotal;

            //get discounts for the order total
            var discountAmountTotal =
                GetOrderTotalDiscount(customer, total, out var orderAppliedDiscounts);
            if (total < discountAmountTotal)
                discountAmountTotal = total;
            total -= discountAmountTotal;

            //applied giftcards
            foreach (var giftCard in _giftCardService.GetAllGiftCards(usedWithOrderId: updatedOrder.Id))
            {
                if (total <= decimal.Zero)
                    continue;

                var remainingAmount = giftCard.GiftCardUsageHistory
                    .Where(history => history.UsedWithOrderId == updatedOrder.Id).Sum(history => history.UsedValue);
                var amountCanBeUsed = total > remainingAmount ? remainingAmount : total;
                total -= amountCanBeUsed;
            }

            //reward points
            var rewardPointsOfOrder = _rewardPointService.GetRewardPointsHistory(customer.Id)
                .FirstOrDefault(history => history.UsedWithOrder == updatedOrder);
            if (rewardPointsOfOrder != null)
            {
                var rewardPoints = -rewardPointsOfOrder.Points;
                var rewardPointsAmount = ConvertRewardPointsToAmount(rewardPoints);
                if (total < rewardPointsAmount)
                {
                    rewardPoints = ConvertAmountToRewardPoints(total);
                    rewardPointsAmount = total;
                }

                if (total > decimal.Zero)
                    total -= rewardPointsAmount;

                //uncomment here for the return unused reward points if new order total less redeemed reward points amount
                //if (rewardPoints < -rewardPointsOfOrder.Points)
                //    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfOrder.Points - rewardPoints, _storeContext.CurrentStore.Id, "Return unused reward points");

                if (rewardPointsAmount != rewardPointsOfOrder.UsedAmount)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPointsAmount;
                    rewardPointsOfOrder.Points = -rewardPoints;
                    _rewardPointService.UpdateRewardPointsHistoryEntry(rewardPointsOfOrder);
                }
            }

            //rounding
            if (total < decimal.Zero)
                total = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                total = _priceCalculationService.RoundPrice(total);

            updatedOrder.OrderDiscount = discountAmountTotal;
            updatedOrder.OrderTotal = total;

            foreach (var discount in orderAppliedDiscounts)
                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);
        }

        /// <summary>
        /// Update tax rates
        /// </summary>
        /// <param name="subTotalTaxRates">Subtotal tax rates</param>
        /// <param name="shippingTotalInclTax">Shipping (incl tax)</param>
        /// <param name="shippingTotalExclTax">Shipping (excl tax)</param>
        /// <param name="shippingTaxRate">Shipping tax rates</param>
        /// <param name="updatedOrder">Order</param>
        /// <returns>Tax total</returns>
        protected virtual decimal UpdateTaxRates(SortedDictionary<decimal, decimal> subTotalTaxRates, decimal shippingTotalInclTax,
            decimal shippingTotalExclTax, decimal shippingTaxRate, Order updatedOrder)
        {
            var taxRates = new SortedDictionary<decimal, decimal>();

            //order subtotal taxes
            var subTotalTax = decimal.Zero;
            foreach (var kvp in subTotalTaxRates)
            {
                subTotalTax += kvp.Value;
                if (kvp.Key <= decimal.Zero || kvp.Value <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(kvp.Key))
                    taxRates.Add(kvp.Key, kvp.Value);
                else
                    taxRates[kvp.Key] = taxRates[kvp.Key] + kvp.Value;
            }

            //shipping taxes
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                shippingTax = shippingTotalInclTax - shippingTotalExclTax;
                if (shippingTax < decimal.Zero)
                    shippingTax = decimal.Zero;

                if (shippingTaxRate > decimal.Zero && shippingTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(shippingTaxRate))
                        taxRates.Add(shippingTaxRate, shippingTax);
                    else
                        taxRates[shippingTaxRate] = taxRates[shippingTaxRate] + shippingTax;
                }
            }

            //payment method additional fee tax
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                paymentMethodAdditionalFeeTax = updatedOrder.PaymentMethodAdditionalFeeInclTax - updatedOrder.PaymentMethodAdditionalFeeExclTax;
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                if (updatedOrder.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    var paymentTaxRate = Math.Round(100 * paymentMethodAdditionalFeeTax / updatedOrder.PaymentMethodAdditionalFeeExclTax, 3);
                    if (paymentTaxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(paymentTaxRate))
                            taxRates.Add(paymentTaxRate, paymentMethodAdditionalFeeTax);
                        else
                            taxRates[paymentTaxRate] = taxRates[paymentTaxRate] + paymentMethodAdditionalFeeTax;
                    }
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTax + shippingTax + paymentMethodAdditionalFeeTax;
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = _priceCalculationService.RoundPrice(taxTotal);

            updatedOrder.OrderTax = taxTotal;
            updatedOrder.TaxRates = taxRates.Aggregate(string.Empty, (current, next) =>
                $"{current}{next.Key.ToString(CultureInfo.InvariantCulture)}:{next.Value.ToString(CultureInfo.InvariantCulture)};   ");
            return taxTotal;
        }

        /// <summary>
        /// Update shipping
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="restoredCart">Cart</param>
        /// <param name="subTotalInclTax">Subtotal (incl tax)</param>
        /// <param name="subTotalExclTax">Subtotal (excl tax)</param>
        /// <param name="updatedOrder">Order</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotalInclTax">Shipping (incl tax)</param>
        /// <param name="shippingTaxRate">Shipping tax rate</param>
        /// <returns>Shipping total</returns>
        protected virtual decimal UpdateShipping(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart,
            decimal subTotalInclTax, decimal subTotalExclTax, Order updatedOrder, Customer customer, out decimal shippingTotalInclTax, out decimal shippingTaxRate)
        {
            var shippingTotalExclTax = decimal.Zero;
            shippingTotalInclTax = decimal.Zero;
            shippingTaxRate = decimal.Zero;

            if (_shoppingCartService.ShoppingCartRequiresShipping(restoredCart))
            {
                if (!IsFreeShipping(restoredCart, _shippingSettings.FreeShippingOverXIncludingTax ? subTotalInclTax : subTotalExclTax))
                {
                    var shippingTotal = decimal.Zero;
                    if (!string.IsNullOrEmpty(updatedOrder.ShippingRateComputationMethodSystemName))
                    {
                        //in the updated order were shipping items
                        if (updatedOrder.PickupInStore)
                        {
                            //customer chose pickup in store method, try to get chosen pickup point
                            if (_shippingSettings.AllowPickupInStore)
                            {
                                var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress, updatedOrder.Customer,
                                    updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                                if (pickupPointsResponse.Success)
                                {
                                    var selectedPickupPoint =
                                        pickupPointsResponse.PickupPoints.FirstOrDefault(point =>
                                            updatedOrder.ShippingMethod.Contains(point.Name));
                                    if (selectedPickupPoint != null)
                                        shippingTotal = selectedPickupPoint.PickupFee;
                                    else
                                        updateOrderParameters.Warnings.Add(
                                            $"Shipping method {updatedOrder.ShippingMethod} could not be loaded");
                                }
                                else
                                    updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
                            }
                            else
                                updateOrderParameters.Warnings.Add("Pick up in store is not available");
                        }
                        else
                        {
                            //customer chose shipping to address, try to get chosen shipping option
                            var shippingOptionsResponse = _shippingService.GetShippingOptions(restoredCart, updatedOrder.ShippingAddress, updatedOrder.Customer, updatedOrder.ShippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id);
                            if (shippingOptionsResponse.Success)
                            {
                                var shippingOption = shippingOptionsResponse.ShippingOptions.FirstOrDefault(option =>
                                    updatedOrder.ShippingMethod.Contains(option.Name));
                                if (shippingOption != null)
                                    shippingTotal = shippingOption.Rate;
                                else
                                    updateOrderParameters.Warnings.Add(
                                        $"Shipping method {updatedOrder.ShippingMethod} could not be loaded");
                            }
                            else
                                updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
                        }
                    }
                    else
                    {
                        //before updating order was without shipping
                        if (_shippingSettings.AllowPickupInStore)
                        {
                            //try to get the cheapest pickup point
                            var pickupPointsResponse = _shippingService.GetPickupPoints(updatedOrder.BillingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                            if (pickupPointsResponse.Success)
                            {
                                updateOrderParameters.PickupPoint = pickupPointsResponse.PickupPoints
                                    .OrderBy(point => point.PickupFee).First();
                                shippingTotal = updateOrderParameters.PickupPoint.PickupFee;
                            }
                            else
                                updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
                        }
                        else
                            updateOrderParameters.Warnings.Add("Pick up in store is not available");

                        if (updateOrderParameters.PickupPoint == null)
                        {
                            //or try to get the cheapest shipping option for the shipping to the customer address 
                            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                            if (shippingRateComputationMethods.Any())
                            {
                                var shippingOptionsResponse = _shippingService.GetShippingOptions(restoredCart, customer.ShippingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                                if (shippingOptionsResponse.Success)
                                {
                                    var shippingOption = shippingOptionsResponse.ShippingOptions.OrderBy(option => option.Rate)
                                        .First();
                                    updatedOrder.ShippingRateComputationMethodSystemName =
                                        shippingOption.ShippingRateComputationMethodSystemName;
                                    updatedOrder.ShippingMethod = shippingOption.Name;
                                    updatedOrder.ShippingAddress = (Address)customer.ShippingAddress.Clone();
                                    shippingTotal = shippingOption.Rate;
                                }
                                else
                                    updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
                            }
                            else
                                updateOrderParameters.Warnings.Add("Shipping rate computation method could not be loaded");
                        }
                    }

                    //additional shipping charge
                    shippingTotal += GetShoppingCartAdditionalShippingCharge(restoredCart);

                    //shipping discounts
                    shippingTotal -= GetShippingDiscount(customer, shippingTotal, out var shippingTotalDiscounts);
                    if (shippingTotal < decimal.Zero)
                        shippingTotal = decimal.Zero;

                    shippingTotalExclTax = _taxService.GetShippingPrice(shippingTotal, false, customer);
                    shippingTotalInclTax = _taxService.GetShippingPrice(shippingTotal, true, customer, out shippingTaxRate);

                    //rounding
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    {
                        shippingTotalExclTax = _priceCalculationService.RoundPrice(shippingTotalExclTax);
                        shippingTotalInclTax = _priceCalculationService.RoundPrice(shippingTotalInclTax);
                    }

                    //change shipping status
                    if (updatedOrder.ShippingStatus == ShippingStatus.ShippingNotRequired ||
                        updatedOrder.ShippingStatus == ShippingStatus.NotYetShipped)
                        updatedOrder.ShippingStatus = ShippingStatus.NotYetShipped;
                    else
                        updatedOrder.ShippingStatus = ShippingStatus.PartiallyShipped;

                    foreach (var discount in shippingTotalDiscounts)
                        if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                            updateOrderParameters.AppliedDiscounts.Add(discount);
                }
            }
            else
                updatedOrder.ShippingStatus = ShippingStatus.ShippingNotRequired;

            updatedOrder.OrderShippingExclTax = shippingTotalExclTax;
            updatedOrder.OrderShippingInclTax = shippingTotalInclTax;
            return shippingTotalExclTax;
        }

        /// <summary>
        /// Update order parameters
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="restoredCart">Cart</param>
        /// <param name="updatedOrderItem">Order item</param>
        /// <param name="updatedOrder">Order</param>
        /// <param name="customer">customer</param>
        /// <param name="subTotalInclTax">Subtotal (incl tax)</param>
        /// <param name="subTotalTaxRates">Subtotal tax rates</param>
        /// <param name="discountAmountExclTax">Discount amount (excl tax)</param>
        /// <returns>Subtotal</returns>
        protected virtual decimal UpdateSubTotal(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart,
            OrderItem updatedOrderItem, Order updatedOrder, Customer customer,
            out decimal subTotalInclTax, out SortedDictionary<decimal, decimal> subTotalTaxRates, out decimal discountAmountExclTax)
        {
            var subTotalExclTax = decimal.Zero;
            subTotalInclTax = decimal.Zero;
            subTotalTaxRates = new SortedDictionary<decimal, decimal>();

            foreach (var shoppingCartItem in restoredCart)
            {
                decimal itemSubTotalExclTax;
                decimal itemSubTotalInclTax;
                decimal taxRate;

                //calculate subtotal for the updated order item
                if (shoppingCartItem.Id == updatedOrderItem.Id)
                {
                    //update order item 
                    updatedOrderItem.UnitPriceExclTax = updateOrderParameters.PriceExclTax;
                    updatedOrderItem.UnitPriceInclTax = updateOrderParameters.PriceInclTax;
                    updatedOrderItem.DiscountAmountExclTax = updateOrderParameters.DiscountAmountExclTax;
                    updatedOrderItem.DiscountAmountInclTax = updateOrderParameters.DiscountAmountInclTax;
                    updatedOrderItem.PriceExclTax = itemSubTotalExclTax = updateOrderParameters.SubTotalExclTax;
                    updatedOrderItem.PriceInclTax = itemSubTotalInclTax = updateOrderParameters.SubTotalInclTax;
                    updatedOrderItem.Quantity = shoppingCartItem.Quantity;

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : taxRate = 0M;
                }
                else
                {
                    //get the already calculated subtotal from the order item
                    itemSubTotalExclTax = updatedOrder.OrderItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceExclTax;
                    itemSubTotalInclTax = updatedOrder.OrderItems.FirstOrDefault(item => item.Id == shoppingCartItem.Id).PriceInclTax;

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : taxRate = 0M;
                }

                subTotalExclTax += itemSubTotalExclTax;
                subTotalInclTax += itemSubTotalInclTax;

                //tax rates
                var itemTaxValue = itemSubTotalInclTax - itemSubTotalExclTax;
                if (taxRate <= decimal.Zero || itemTaxValue <= decimal.Zero)
                    continue;

                if (!subTotalTaxRates.ContainsKey(taxRate))
                    subTotalTaxRates.Add(taxRate, itemTaxValue);
                else
                    subTotalTaxRates[taxRate] = subTotalTaxRates[taxRate] + itemTaxValue;
            }

            if (subTotalExclTax < decimal.Zero)
                subTotalExclTax = decimal.Zero;

            if (subTotalInclTax < decimal.Zero)
                subTotalInclTax = decimal.Zero;

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTax, out var subTotalDiscounts);
            if (subTotalExclTax < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTax;
            var discountAmountInclTax = discountAmountExclTax;

            //add tax for shopping items
            var tempTaxRates = new Dictionary<decimal, decimal>(subTotalTaxRates);
            foreach (var kvp in tempTaxRates)
            {
                if (kvp.Value == decimal.Zero || subTotalExclTax <= decimal.Zero)
                    continue;

                var discountTaxValue = kvp.Value * (discountAmountExclTax / subTotalExclTax);
                discountAmountInclTax += discountTaxValue;
                subTotalTaxRates[kvp.Key] = kvp.Value - discountTaxValue;
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalExclTax = _priceCalculationService.RoundPrice(subTotalExclTax);
                subTotalInclTax = _priceCalculationService.RoundPrice(subTotalInclTax);
                discountAmountExclTax = _priceCalculationService.RoundPrice(discountAmountExclTax);
                discountAmountInclTax = _priceCalculationService.RoundPrice(discountAmountInclTax);
            }

            updatedOrder.OrderSubtotalExclTax = subTotalExclTax;
            updatedOrder.OrderSubtotalInclTax = subTotalInclTax;
            updatedOrder.OrderSubTotalDiscountExclTax = discountAmountExclTax;
            updatedOrder.OrderSubTotalDiscountInclTax = discountAmountInclTax;

            foreach (var discount in subTotalDiscounts)
                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);
            return subTotalExclTax;
        }

        /// <summary>
        /// Set reward points
        /// </summary>
        /// <param name="redeemedRewardPoints">Redeemed reward points</param>
        /// <param name="redeemedRewardPointsAmount">Redeemed reward points amount</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        protected virtual void SetRewardPoints(ref int redeemedRewardPoints, ref decimal redeemedRewardPointsAmount,
            bool? useRewardPoints, Customer customer, decimal orderTotal)
        {
            if (!_rewardPointsSettings.Enabled)
                return;

            if (!useRewardPoints.HasValue)
                useRewardPoints = _genericAttributeService.GetAttribute<bool>(customer, NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, _storeContext.CurrentStore.Id);

            if (!useRewardPoints.Value)
                return;

            var rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            rewardPointsBalance = _rewardPointService.GetReducedPointsBalance(rewardPointsBalance);

            if (!CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                return;

            var rewardPointsBalanceAmount = ConvertRewardPointsToAmount(rewardPointsBalance);

            if (orderTotal <= decimal.Zero)
                return;

            if (orderTotal > rewardPointsBalanceAmount)
            {
                redeemedRewardPoints = rewardPointsBalance;
                redeemedRewardPointsAmount = rewardPointsBalanceAmount;
            }
            else
            {
                redeemedRewardPointsAmount = orderTotal;
                redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
            }
        }

        /// <summary>
        /// Apply gift cards
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="customer">Customer</param>
        /// <param name="resultTemp"></param>
        protected virtual void AppliedGiftCards(IList<ShoppingCartItem> cart, List<AppliedGiftCard> appliedGiftCards,
            Customer customer, ref decimal resultTemp)
        {
            if (_shoppingCartService.ShoppingCartIsRecurring(cart))
                return;

            //we don't apply gift cards for recurring products
            var giftCards = _giftCardService.GetActiveGiftCardsAppliedByCustomer(customer);
            if (giftCards == null)
                return;

            foreach (var gc in giftCards)
            {
                if (resultTemp <= decimal.Zero)
                    continue;

                var remainingAmount = _giftCardService.GetGiftCardRemainingAmount(gc);
                var amountCanBeUsed = resultTemp > remainingAmount ? remainingAmount : resultTemp;

                //reduce subtotal
                resultTemp -= amountCanBeUsed;

                var appliedGiftCard = new AppliedGiftCard
                {
                    GiftCard = gc,
                    AmountCanBeUsed = amountCanBeUsed
                };
                appliedGiftCards.Add(appliedGiftCard);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart, bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount)
        {
            GetShoppingCartSubTotal(cart, includingTax,
                out discountAmount, out appliedDiscounts,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out _);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        public virtual void GetShoppingCartSubTotal(IList<ShoppingCartItem> cart,
            bool includingTax,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates)
        {
            discountAmount = decimal.Zero;
            appliedDiscounts = new List<DiscountForCaching>();
            subTotalWithoutDiscount = decimal.Zero;
            subTotalWithDiscount = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            if (!cart.Any())
                return;

            //get the customer 
            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;

            //sub totals
            var subTotalExclTaxWithoutDiscount = decimal.Zero;
            var subTotalInclTaxWithoutDiscount = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                var sciSubTotal = _priceCalculationService.GetSubTotal(shoppingCartItem);

                var sciExclTax = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, false, customer, out var taxRate);
                var sciInclTax = _taxService.GetProductPrice(shoppingCartItem.Product, sciSubTotal, true, customer, out taxRate);
                subTotalExclTaxWithoutDiscount += sciExclTax;
                subTotalInclTaxWithoutDiscount += sciInclTax;

                //tax rates
                var sciTax = sciInclTax - sciExclTax;
                if (taxRate <= decimal.Zero || sciTax <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(taxRate))
                {
                    taxRates.Add(taxRate, sciTax);
                }
                else
                {
                    taxRates[taxRate] = taxRates[taxRate] + sciTax;
                }
            }

            //checkout attributes
            if (customer != null)
            {
                var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        var caExclTax = _taxService.GetCheckoutAttributePrice(attributeValue, false, customer, out var taxRate);
                        var caInclTax = _taxService.GetCheckoutAttributePrice(attributeValue, true, customer, out taxRate);
                        subTotalExclTaxWithoutDiscount += caExclTax;
                        subTotalInclTaxWithoutDiscount += caInclTax;

                        //tax rates
                        var caTax = caInclTax - caExclTax;
                        if (taxRate <= decimal.Zero || caTax <= decimal.Zero)
                            continue;

                        if (!taxRates.ContainsKey(taxRate))
                        {
                            taxRates.Add(taxRate, caTax);
                        }
                        else
                        {
                            taxRates[taxRate] = taxRates[taxRate] + caTax;
                        }
                    }
                }
            }

            //subtotal without discount
            subTotalWithoutDiscount = includingTax ? subTotalInclTaxWithoutDiscount : subTotalExclTaxWithoutDiscount;
            if (subTotalWithoutDiscount < decimal.Zero)
                subTotalWithoutDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithoutDiscount = _priceCalculationService.RoundPrice(subTotalWithoutDiscount);

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            var discountAmountExclTax = GetOrderSubtotalDiscount(customer, subTotalExclTaxWithoutDiscount, out appliedDiscounts);
            if (subTotalExclTaxWithoutDiscount < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTaxWithoutDiscount;
            var discountAmountInclTax = discountAmountExclTax;
            //subtotal with discount (excl tax)
            var subTotalExclTaxWithDiscount = subTotalExclTaxWithoutDiscount - discountAmountExclTax;
            var subTotalInclTaxWithDiscount = subTotalExclTaxWithDiscount;

            //add tax for shopping items & checkout attributes
            var tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
            foreach (var kvp in tempTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;

                if (taxValue == decimal.Zero)
                    continue;

                //discount the tax amount that applies to subtotal items
                if (subTotalExclTaxWithoutDiscount > decimal.Zero)
                {
                    var discountTax = taxRates[taxRate] * (discountAmountExclTax / subTotalExclTaxWithoutDiscount);
                    discountAmountInclTax += discountTax;
                    taxValue = taxRates[taxRate] - discountTax;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        taxValue = _priceCalculationService.RoundPrice(taxValue);
                    taxRates[taxRate] = taxValue;
                }

                //subtotal with discount (incl tax)
                subTotalInclTaxWithDiscount += taxValue;
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                discountAmountInclTax = _priceCalculationService.RoundPrice(discountAmountInclTax);
                discountAmountExclTax = _priceCalculationService.RoundPrice(discountAmountExclTax);
            }

            if (includingTax)
            {
                subTotalWithDiscount = subTotalInclTaxWithDiscount;
                discountAmount = discountAmountInclTax;
            }
            else
            {
                subTotalWithDiscount = subTotalExclTaxWithDiscount;
                discountAmount = discountAmountExclTax;
            }

            if (subTotalWithDiscount < decimal.Zero)
                subTotalWithDiscount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                subTotalWithDiscount = _priceCalculationService.RoundPrice(subTotalWithDiscount);
        }

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        public virtual void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

            //get the customer 
            var customer = restoredCart.FirstOrDefault(item => item.Customer != null)?.Customer;

            //sub total
            var subTotalExclTax = UpdateSubTotal(updateOrderParameters, restoredCart, updatedOrderItem, updatedOrder, customer, out var subTotalInclTax, out var subTotalTaxRates, out var discountAmountExclTax);

            //shipping
            var shippingTotalExclTax = UpdateShipping(updateOrderParameters, restoredCart, subTotalInclTax, subTotalExclTax, updatedOrder, customer, out var shippingTotalInclTax, out var shippingTaxRate);

            //tax rates
            var taxTotal = UpdateTaxRates(subTotalTaxRates, shippingTotalInclTax, shippingTotalExclTax, shippingTaxRate, updatedOrder);

            //total
            UpdateTotal(updateOrderParameters, subTotalExclTax, discountAmountExclTax, shippingTotalExclTax, updatedOrder, taxTotal, customer);
        }

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Additional shipping charge</returns>
        public virtual decimal GetShoppingCartAdditionalShippingCharge(IList<ShoppingCartItem> cart)
        {
            return cart.Sum(shoppingCartItem => _shippingService.GetAdditionalShippingCharge(shoppingCartItem));
        }

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>A value indicating whether shipping is free</returns>
        public virtual bool IsFreeShipping(IList<ShoppingCartItem> cart, decimal? subTotal = null)
        {
            //check whether customer is in a customer role with free shipping applied
            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            if (customer != null && customer.CustomerRoles.Where(role => role.Active).Any(role => role.FreeShipping))
                return true;

            //check whether all shopping cart items and their associated products marked as free shipping
            if (cart.All(shoppingCartItem => _shippingService.IsFreeShipping(shoppingCartItem)))
                return true;

            //free shipping over $X
            if (!_shippingSettings.FreeShippingOverXEnabled)
                return false;

            if (!subTotal.HasValue)
            {
                GetShoppingCartSubTotal(cart, _shippingSettings.FreeShippingOverXIncludingTax, out _, out _, out _, out var subTotalWithDiscount);
                subTotal = subTotalWithDiscount;
            }

            //check whether we have subtotal enough to have free shipping
            if (subTotal.Value > _shippingSettings.FreeShippingOverXValue)
                return true;

            return false;
        }

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Adjusted shipping rate</returns>
        public virtual decimal AdjustShippingRate(decimal shippingRate,
            IList<ShoppingCartItem> cart, out List<DiscountForCaching> appliedDiscounts)
        {
            appliedDiscounts = new List<DiscountForCaching>();

            //free shipping
            if (IsFreeShipping(cart))
                return decimal.Zero;

            //with additional shipping charges
            var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(cart.FirstOrDefault(item => item.Customer != null)?.Customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.CurrentStore.Id);

            var adjustedRate = shippingRate;

            if (!_shippingSettings.AllowPickupInStore || pickupPoint == null || !_shippingSettings.IgnoreAdditionalShippingChargeForPickupInStore)
            {
                adjustedRate += GetShoppingCartAdditionalShippingCharge(cart);
            }

            //discount
            var discountAmount = GetShippingDiscount(cart.FirstOrDefault(item => item.Customer != null)?.Customer, adjustedRate, out appliedDiscounts);
            adjustedRate -= discountAmount;

            adjustedRate = Math.Max(adjustedRate, decimal.Zero);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = _priceCalculationService.RoundPrice(adjustedRate);

            return adjustedRate;
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods)
        {
            var includingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return GetShoppingCartShippingTotal(cart, includingTax, shippingRateComputationMethods);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods)
        {
            return GetShoppingCartShippingTotal(cart, includingTax, shippingRateComputationMethods, out _);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods,
            out decimal taxRate)
        {
            return GetShoppingCartShippingTotal(cart, includingTax, shippingRateComputationMethods, out taxRate, out _);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Shipping total</returns>
        public virtual decimal? GetShoppingCartShippingTotal(IList<ShoppingCartItem> cart, bool includingTax, IList<IShippingRateComputationMethod> shippingRateComputationMethods,
            out decimal taxRate, out List<DiscountForCaching> appliedDiscounts)
        {
            decimal? shippingTotal = null;
            decimal? shippingTotalTaxed = null;
            appliedDiscounts = new List<DiscountForCaching>();
            taxRate = decimal.Zero;

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;

            var isFreeShipping = IsFreeShipping(cart);
            if (isFreeShipping)
                return decimal.Zero;

            ShippingOption shippingOption = null;
            if (customer != null)
                shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);

            if (shippingOption != null)
            {
                //use last shipping option (get from cache)
                shippingTotal = AdjustShippingRate(shippingOption.Rate, cart, out appliedDiscounts);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = customer.ShippingAddress;

                if (shippingRateComputationMethods != null)
                {
                    if (!shippingRateComputationMethods.Any() && !_shippingSettings.AllowPickupInStore)
                        throw new NopException("Shipping rate computation method could not be loaded");

                    if (shippingRateComputationMethods.Count == 1)
                    {
                        var shippingRateComputationMethod = shippingRateComputationMethods[0];

                        var shippingOptionRequests = _shippingService.CreateShippingOptionRequests(cart,
                            shippingAddress,
                            _storeContext.CurrentStore.Id,
                            out _);
                        decimal? fixedRate = null;
                        foreach (var shippingOptionRequest in shippingOptionRequests)
                        {
                            //calculate fixed rates for each request-package
                            var fixedRateTmp = shippingRateComputationMethod.GetFixedRate(shippingOptionRequest);
                            if (!fixedRateTmp.HasValue)
                                continue;

                            if (!fixedRate.HasValue)
                                fixedRate = decimal.Zero;

                            fixedRate += fixedRateTmp.Value;
                        }

                        if (fixedRate.HasValue)
                        {
                            //adjust shipping rate
                            shippingTotal = AdjustShippingRate(fixedRate.Value, cart, out appliedDiscounts);
                        }
                    }
                }
            }

            if (!shippingTotal.HasValue)
                return null;

            if (shippingTotal.Value < decimal.Zero)
                shippingTotal = decimal.Zero;

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotal = _priceCalculationService.RoundPrice(shippingTotal.Value);

            shippingTotalTaxed = _taxService.GetShippingPrice(shippingTotal.Value,
                includingTax,
                customer,
                out taxRate);

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotalTaxed = _priceCalculationService.RoundPrice(shippingTotalTaxed.Value);

            return shippingTotalTaxed;
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods = null, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            return GetTaxTotal(cart, shippingRateComputationMethods, out _, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingRateComputationMethods">Shipping rate computation methods</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Tax total</returns>
        public virtual decimal GetTaxTotal(IList<ShoppingCartItem> cart, IList<IShippingRateComputationMethod> shippingRateComputationMethods,
            out SortedDictionary<decimal, decimal> taxRates, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            taxRates = new SortedDictionary<decimal, decimal>();

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            var paymentMethodSystemName = string.Empty;
            if (customer != null)
            {
                paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            }

            //order sub total (items + checkout attributes)
            var subTotalTaxTotal = decimal.Zero;
            GetShoppingCartSubTotal(cart, false, out _, out _, out _, out _, out var orderSubTotalTaxRates);
            foreach (var kvp in orderSubTotalTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate <= decimal.Zero || taxValue <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(taxRate))
                    taxRates.Add(taxRate, taxValue);
                else
                    taxRates[taxRate] = taxRates[taxRate] + taxValue;
            }

            //shipping
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                var shippingExclTax = GetShoppingCartShippingTotal(cart, false, shippingRateComputationMethods, out var taxRate);
                var shippingInclTax = GetShoppingCartShippingTotal(cart, true, shippingRateComputationMethods, out taxRate);
                if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                {
                    shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                    //ensure that tax is equal or greater than zero
                    if (shippingTax < decimal.Zero)
                        shippingTax = decimal.Zero;

                    //tax rates
                    if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                            taxRates.Add(taxRate, shippingTax);
                        else
                            taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                    }
                }
            }

            //payment method additional fee
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out var taxRate);
                var paymentMethodAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                //ensure that tax is equal or greater than zero
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                //tax rates
                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
            //ensure that tax is equal or greater than zero
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;
            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = _priceCalculationService.RoundPrice(taxTotal);
            return taxTotal;
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            return GetShoppingCartTotal(cart, out _, out _, out _, out _, out _, useRewardPoints, usePaymentMethodAdditionalFee);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        public virtual decimal? GetShoppingCartTotal(IList<ShoppingCartItem> cart,
            out decimal discountAmount, out List<DiscountForCaching> appliedDiscounts,
            out List<AppliedGiftCard> appliedGiftCards,
            out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            redeemedRewardPoints = 0;
            redeemedRewardPointsAmount = decimal.Zero;

            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            var paymentMethodSystemName = string.Empty;
            if (customer != null)
            {
                paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            }

            //subtotal without tax
            GetShoppingCartSubTotal(cart, false, out _, out _, out _, out var subTotalWithDiscountBase);
            //subtotal with discount
            var subtotalBase = subTotalWithDiscountBase;

            //LoadAllShippingRateComputationMethods
            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            //shipping without tax
            var shoppingCartShipping = GetShoppingCartShippingTotal(cart, false, shippingRateComputationMethods);

            //payment method additional fee without tax
            var paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !string.IsNullOrEmpty(paymentMethodSystemName))
            {
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart,
                    paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax =
                    _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee,
                        false, customer);
            }

            //tax
            var shoppingCartTax = GetTaxTotal(cart, shippingRateComputationMethods, usePaymentMethodAdditionalFee);

            //order total
            var resultTemp = decimal.Zero;
            resultTemp += subtotalBase;
            if (shoppingCartShipping.HasValue)
            {
                resultTemp += shoppingCartShipping.Value;
            }

            resultTemp += paymentMethodAdditionalFeeWithoutTax;
            resultTemp += shoppingCartTax;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _priceCalculationService.RoundPrice(resultTemp);

            //order total discount
            discountAmount = GetOrderTotalDiscount(customer, resultTemp, out appliedDiscounts);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _priceCalculationService.RoundPrice(resultTemp);

            //let's apply gift cards now (gift cards that can be used)
            appliedGiftCards = new List<AppliedGiftCard>();
            AppliedGiftCards(cart, appliedGiftCards, customer, ref resultTemp);

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = _priceCalculationService.RoundPrice(resultTemp);

            if (!shoppingCartShipping.HasValue)
            {
                //we have errors
                return null;
            }

            var orderTotal = resultTemp;

            //reward points
            SetRewardPoints(ref redeemedRewardPoints, ref redeemedRewardPointsAmount, useRewardPoints, customer, orderTotal);

            orderTotal = orderTotal - redeemedRewardPointsAmount;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                orderTotal = _priceCalculationService.RoundPrice(orderTotal);
            return orderTotal;
        }

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = _priceCalculationService.RoundPrice(result);
            return result;
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            var result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        /// <summary>
        /// Calculate how order total (maximum amount) for which reward points could be earned/reduced
        /// </summary>
        /// <param name="orderShippingInclTax">Order shipping (including tax)</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>Applicable order total</returns>
        public virtual decimal CalculateApplicableOrderTotalForRewardPoints(decimal orderShippingInclTax, decimal orderTotal)
        {
            //do you give reward points for order total? or do you exclude shipping?
            //since shipping costs vary some of store owners don't give reward points based on shipping total
            //you can put your custom logic here
            var totalForRewardPoints = orderTotal - orderShippingInclTax;

            //check the minimum total to award points
            if (totalForRewardPoints < _rewardPointsSettings.MinOrderTotalToAwardPoints)
                return decimal.Zero;

            return totalForRewardPoints;
        }

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>Number of reward points</returns>
        public virtual int CalculateRewardPoints(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || customer.IsGuest())
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
            return points;
        }

        #endregion
    }
}