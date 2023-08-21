using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
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

        protected readonly CatalogSettings _catalogSettings;
        protected readonly IAddressService _addressService;
        protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
        protected readonly ICustomerService _customerService;
        protected readonly IDiscountService _discountService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IGiftCardService _giftCardService;
        protected readonly IOrderService _orderService;
        protected readonly IPaymentService _paymentService;
        protected readonly IPriceCalculationService _priceCalculationService;
        protected readonly IProductService _productService;
        protected readonly IRewardPointService _rewardPointService;
        protected readonly IShippingPluginManager _shippingPluginManager;
        protected readonly IShippingService _shippingService;
        protected readonly IShoppingCartService _shoppingCartService;
        protected readonly IStoreContext _storeContext;
        protected readonly ITaxService _taxService;
        protected readonly IWorkContext _workContext;
        protected readonly RewardPointsSettings _rewardPointsSettings;
        protected readonly ShippingSettings _shippingSettings;
        protected readonly ShoppingCartSettings _shoppingCartSettings;
        protected readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OrderTotalCalculationService(CatalogSettings catalogSettings,
            IAddressService addressService,
            IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
            ICustomerService customerService,
            IDiscountService discountService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
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
            _addressService = addressService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _customerService = customerService;
            _discountService = discountService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _orderService = orderService;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order discount, Applied discounts
        /// </returns>
        protected virtual async Task<(decimal orderDiscount, List<Discount> appliedDiscounts)> GetOrderSubtotalDiscountAsync(Customer customer,
            decimal orderSubTotal)
        {
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (discountAmount, new List<Discount>());

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
            {
                var couponCodesToValidate = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);
                foreach (var discount in allDiscounts)
                {
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer, couponCodesToValidate)).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }
                }
            }

            var appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderSubTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return (discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping discount. Applied discounts
        /// </returns>
        protected virtual async Task<(decimal shippingDiscount, List<Discount> appliedDiscounts)> GetShippingDiscountAsync(Customer customer, decimal shippingTotal)
        {
            var appliedDiscounts = new List<Discount>();
            var shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (shippingDiscountAmount, appliedDiscounts);

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
            {
                var couponCodesToValidate = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);

                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer, couponCodesToValidate)).IsValid)
                        allowedDiscounts.Add(discount);
            }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = await _priceCalculationService.RoundPriceAsync(shippingDiscountAmount);

            return (shippingDiscountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order discount. Applied discounts
        /// </returns>
        protected virtual async Task<(decimal orderDiscount, List<Discount> appliedDiscounts)> GetOrderTotalDiscountAsync(Customer customer, decimal orderTotal)
        {
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (discountAmount, new List<Discount>());

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
            {
                var couponCodesToValidate = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);
                foreach (var discount in allDiscounts)
                {
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer, couponCodesToValidate)).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }
                }
            }

            var appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = await _priceCalculationService.RoundPriceAsync(discountAmount);

            return (discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Update order total
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="subTotalExclTax">Subtotal (excl tax)</param>
        /// <param name="discountAmountExclTax">Discount amount (excl tax)</param>
        /// <param name="shippingTotalExclTax">Shipping (excl tax)</param>
        /// <param name="taxTotal">Tax</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateTotalAsync(UpdateOrderParameters updateOrderParameters, decimal subTotalExclTax,
            decimal discountAmountExclTax, decimal shippingTotalExclTax, decimal taxTotal)
        {
            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);

            var total = subTotalExclTax - discountAmountExclTax + shippingTotalExclTax + updatedOrder.PaymentMethodAdditionalFeeExclTax + taxTotal;

            //get discounts for the order total
            var (discountAmountTotal, orderAppliedDiscounts) = await GetOrderTotalDiscountAsync(customer, total);
            if (total < discountAmountTotal)
                discountAmountTotal = total;
            total -= discountAmountTotal;

            //applied giftcards
            foreach (var giftCard in await _giftCardService.GetAllGiftCardsAsync(usedWithOrderId: updatedOrder.Id))
            {
                if (total <= decimal.Zero)
                    continue;

                var remainingAmount = (await _giftCardService.GetGiftCardUsageHistoryAsync(giftCard))
                    .Where(history => history.UsedWithOrderId == updatedOrder.Id).Sum(history => history.UsedValue);
                var amountCanBeUsed = total > remainingAmount ? remainingAmount : total;
                total -= amountCanBeUsed;
            }

            //reward points
            var rewardPointsOfOrder = await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(updatedOrder.RedeemedRewardPointsEntryId ?? 0);
            if (rewardPointsOfOrder != null)
            {
                var rewardPoints = -rewardPointsOfOrder.Points;
                var rewardPointsAmount = await ConvertRewardPointsToAmountAsync(rewardPoints);
                if (total < rewardPointsAmount)
                {
                    rewardPoints = ConvertAmountToRewardPoints(total);
                    rewardPointsAmount = total;
                }

                if (total > decimal.Zero)
                    total -= rewardPointsAmount;

                //uncomment here for the return unused reward points if new order total less redeemed reward points amount
                //if (rewardPoints < -rewardPointsOfOrder.Points)
                //    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfOrder.Points - rewardPoints, store.Id, "Return unused reward points");

                if (rewardPointsAmount != rewardPointsOfOrder.UsedAmount)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPointsAmount;
                    rewardPointsOfOrder.Points = -rewardPoints;
                    await _rewardPointService.UpdateRewardPointsHistoryEntryAsync(rewardPointsOfOrder);
                }
            }

            //rounding
            if (total < decimal.Zero)
                total = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                total = await _priceCalculationService.RoundPriceAsync(total);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total
        /// </returns>
        protected virtual async Task<decimal> UpdateTaxRatesAsync(SortedDictionary<decimal, decimal> subTotalTaxRates, decimal shippingTotalInclTax,
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
                taxTotal = await _priceCalculationService.RoundPriceAsync(taxTotal);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Shipping (incl tax). Shipping tax rate
        /// </returns>
        protected virtual async Task<(decimal shippingTotal, decimal shippingTotalInclTax, decimal shippingTaxRate)> UpdateShippingAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart,
            decimal subTotalInclTax, decimal subTotalExclTax)
        {
            var shippingTotalExclTax = decimal.Zero;
            var shippingTotalInclTax = decimal.Zero;
            var shippingTaxRate = decimal.Zero;

            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(restoredCart))
            {
                if (!await IsFreeShippingAsync(restoredCart, _shippingSettings.FreeShippingOverXIncludingTax ? subTotalInclTax : subTotalExclTax))
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
                                var address = await _addressService.GetAddressByIdAsync(updatedOrder.BillingAddressId);
                                var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(restoredCart, address,
                                    customer, updatedOrder.ShippingRateComputationMethodSystemName, store.Id);
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
                            var shippingAddress = await _addressService.GetAddressByIdAsync(updatedOrder.ShippingAddressId ?? 0);
                            var shippingOptionsResponse = await _shippingService.GetShippingOptionsAsync(restoredCart, shippingAddress, customer, updatedOrder.ShippingRateComputationMethodSystemName, store.Id);
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
                            var address = await _addressService.GetAddressByIdAsync(updatedOrder.BillingAddressId);
                            var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(restoredCart, address,
                                currentCustomer, storeId: store.Id);
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
                            var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync();
                            if (shippingRateComputationMethods.Any())
                            {
                                var customerShippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer);

                                var shippingOptionsResponse = await _shippingService.GetShippingOptionsAsync(restoredCart, customerShippingAddress, currentCustomer, storeId: store.Id);
                                if (shippingOptionsResponse.Success)
                                {
                                    var shippingOption = shippingOptionsResponse.ShippingOptions.OrderBy(option => option.Rate)
                                        .First();
                                    updatedOrder.ShippingRateComputationMethodSystemName =
                                        shippingOption.ShippingRateComputationMethodSystemName;
                                    updatedOrder.ShippingMethod = shippingOption.Name;

                                    var updatedShippingAddress = _addressService.CloneAddress(customerShippingAddress);
                                    await _addressService.InsertAddressAsync(updatedShippingAddress);
                                    updatedOrder.ShippingAddressId = updatedShippingAddress.Id;

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
                    shippingTotal += await GetShoppingCartAdditionalShippingChargeAsync(restoredCart);

                    //shipping discounts
                    var (shippingDiscount, shippingTotalDiscounts) = await GetShippingDiscountAsync(customer, shippingTotal);
                    shippingTotal -= shippingDiscount;
                    if (shippingTotal < decimal.Zero)
                        shippingTotal = decimal.Zero;

                    shippingTotalExclTax = (await _taxService.GetShippingPriceAsync(shippingTotal, false, customer)).price;
                    (shippingTotalInclTax, shippingTaxRate) = await _taxService.GetShippingPriceAsync(shippingTotal, true, customer);

                    //rounding
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                    {
                        shippingTotalExclTax = await _priceCalculationService.RoundPriceAsync(shippingTotalExclTax);
                        shippingTotalInclTax = await _priceCalculationService.RoundPriceAsync(shippingTotalInclTax);
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

            return (shippingTotalExclTax, shippingTotalInclTax, shippingTaxRate);
        }

        /// <summary>
        /// Update order parameters
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="restoredCart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subtotal. Subtotal (incl tax). Subtotal tax rates. Discount amount (excl tax)
        /// </returns>
        protected virtual async Task<(decimal subtotal, decimal subTotalInclTax, SortedDictionary<decimal, decimal> subTotalTaxRates, decimal discountAmountExclTax)> UpdateSubTotalAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            var subTotalExclTax = decimal.Zero;
            var subTotalInclTax = decimal.Zero;
            var subTotalTaxRates = new SortedDictionary<decimal, decimal>();

            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

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

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : 0M;
                }
                else
                {
                    //get the already calculated subtotal from the order item
                    var order = await _orderService.GetOrderItemByIdAsync(shoppingCartItem.Id);
                    itemSubTotalExclTax = order.PriceExclTax;
                    itemSubTotalInclTax = order.PriceInclTax;

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : 0M;
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
            var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);
            var (discountAmountExclTax, subTotalDiscounts) = await GetOrderSubtotalDiscountAsync(customer, subTotalExclTax);
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
                subTotalExclTax = await _priceCalculationService.RoundPriceAsync(subTotalExclTax);
                subTotalInclTax = await _priceCalculationService.RoundPriceAsync(subTotalInclTax);
                discountAmountExclTax = await _priceCalculationService.RoundPriceAsync(discountAmountExclTax);
                discountAmountInclTax = await _priceCalculationService.RoundPriceAsync(discountAmountInclTax);
            }

            updatedOrder.OrderSubtotalExclTax = subTotalExclTax;
            updatedOrder.OrderSubtotalInclTax = subTotalInclTax;
            updatedOrder.OrderSubTotalDiscountExclTax = discountAmountExclTax;
            updatedOrder.OrderSubTotalDiscountInclTax = discountAmountInclTax;

            foreach (var discount in subTotalDiscounts)
                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            return (subTotalExclTax, subTotalInclTax, subTotalTaxRates, discountAmountExclTax);
        }

        /// <summary>
        /// Set reward points
        /// </summary>
        /// <param name="redeemedRewardPoints">Redeemed reward points</param>
        /// <param name="redeemedRewardPointsAmount">Redeemed reward points amount</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(int redeemedRewardPoints, decimal redeemedRewardPointsAmount)> SetRewardPointsAsync(int redeemedRewardPoints, decimal redeemedRewardPointsAmount,
            bool? useRewardPoints, Customer customer, decimal orderTotal)
        {
            if (!_rewardPointsSettings.Enabled)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            var store = await _storeContext.GetCurrentStoreAsync();
            if (!useRewardPoints.HasValue)
                useRewardPoints = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, store.Id);

            if (!useRewardPoints.Value)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            if (orderTotal <= decimal.Zero)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);

            if (_rewardPointsSettings.MaximumRewardPointsToUsePerOrder > 0 && rewardPointsBalance > _rewardPointsSettings.MaximumRewardPointsToUsePerOrder)
                rewardPointsBalance = _rewardPointsSettings.MaximumRewardPointsToUsePerOrder;

            var rewardPointsBalanceAmount = await ConvertRewardPointsToAmountAsync(rewardPointsBalance);

            if (_rewardPointsSettings.MaximumRedeemedRate > 0 && _rewardPointsSettings.MaximumRedeemedRate < rewardPointsBalanceAmount / orderTotal)
            {
                rewardPointsBalance = ConvertAmountToRewardPoints(orderTotal * _rewardPointsSettings.MaximumRedeemedRate);
                rewardPointsBalanceAmount = await ConvertRewardPointsToAmountAsync(rewardPointsBalance);
            }

            if (!CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

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

            return (redeemedRewardPoints, redeemedRewardPointsAmount);
        }

        /// <summary>
        /// Apply gift cards
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="customer">Customer</param>
        /// <param name="resultTemp"></param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<decimal> AppliedGiftCardsAsync(IList<ShoppingCartItem> cart, List<AppliedGiftCard> appliedGiftCards,
            Customer customer, decimal resultTemp)
        {
            if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
                return resultTemp;

            //we don't apply gift cards for recurring products
            var giftCards = await _giftCardService.GetActiveGiftCardsAppliedByCustomerAsync(customer);
            if (giftCards == null)
                return resultTemp;

            foreach (var gc in giftCards)
            {
                if (resultTemp <= decimal.Zero)
                    continue;

                var remainingAmount = await _giftCardService.GetGiftCardRemainingAmountAsync(gc);
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

            return resultTemp;
        }

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional shipping charge
        /// </returns>
        protected virtual async Task<decimal> GetShoppingCartAdditionalShippingChargeAsync(IList<ShoppingCartItem> cart)
        {
            return await cart.SumAwaitAsync(async shoppingCartItem => await _shippingService.GetAdditionalShippingChargeAsync(shoppingCartItem));
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        protected virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            var result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the applied discount amount. Applied discounts. Sub total (without discount). Sub total (with discount). Tax rates (of order sub total)
        /// </returns>
        public virtual async Task<(decimal discountAmount, List<Discount> appliedDiscounts, decimal subTotalWithoutDiscount, decimal subTotalWithDiscount, SortedDictionary<decimal, decimal> taxRates)> GetShoppingCartSubTotalAsync(IList<ShoppingCartItem> cart,
            bool includingTax)
        {
            var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await GetShoppingCartSubTotalsAsync(cart);

            return (includingTax ? discountAmountInclTax : discountAmountExclTax, appliedDiscounts,
                includingTax ? subTotalWithoutDiscountInclTax : subTotalWithoutDiscountExclTax,
                includingTax ? subTotalWithDiscountInclTax : subTotalWithDiscountExclTax, taxRates);
        }

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the applied discount amount. Applied discounts. Sub total (without discount). Sub total (with discount). Tax rates (of order sub total)
        /// </returns>
        public virtual async Task<(decimal discountAmountInclTax, decimal discountAmountExclTax, List<Discount>
            appliedDiscounts, decimal subTotalWithoutDiscountInclTax, decimal subTotalWithoutDiscountExclTax, decimal
            subTotalWithDiscountInclTax, decimal subTotalWithDiscountExclTax, SortedDictionary<decimal, decimal>
            taxRates)> GetShoppingCartSubTotalsAsync(IList<ShoppingCartItem> cart)
        {
            var discountAmountExclTax = decimal.Zero;
            var discountAmountInclTax = decimal.Zero;
            var appliedDiscounts = new List<Discount>();
            var subTotalWithoutDiscountExclTax = decimal.Zero;
            var subTotalWithoutDiscountInclTax = decimal.Zero;

            var subTotalWithDiscountExclTax = decimal.Zero;
            var subTotalWithDiscountInclTax = decimal.Zero;

            var taxRates = new SortedDictionary<decimal, decimal>();

            if (!cart.Any())
                return (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates);

            //get the customer 
            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);

            //sub totals
            foreach (var shoppingCartItem in cart)
            {
                var sciSubTotal = (await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true)).subTotal;
                var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

                var (sciExclTax, taxRate) = await _taxService.GetProductPriceAsync(product, sciSubTotal, false, customer);
                var (sciInclTax, _) = await _taxService.GetProductPriceAsync(product, sciSubTotal, true, customer);

                subTotalWithoutDiscountExclTax += sciExclTax;
                subTotalWithoutDiscountInclTax += sciInclTax;

                //tax rates
                var sciTax = sciInclTax - sciExclTax;
                if (taxRate <= decimal.Zero || sciTax <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(taxRate))
                    taxRates.Add(taxRate, sciTax);
                else
                    taxRates[taxRate] += sciTax;
            }

            //checkout attributes
            if (customer != null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
                var attributeValues = _checkoutAttributeParser.ParseAttributeValues(checkoutAttributesXml);
                if (attributeValues != null)
                {
                    await foreach (var (attribute, values) in attributeValues)
                    {
                        await foreach (var attributeValue in values)
                        {
                            var (caExclTax, taxRate) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, false, customer);
                            var (caInclTax, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, true, customer);

                            subTotalWithoutDiscountExclTax += caExclTax;
                            subTotalWithoutDiscountInclTax += caInclTax;

                            //tax rates
                            var caTax = caInclTax - caExclTax;
                            if (taxRate <= decimal.Zero || caTax <= decimal.Zero)
                                continue;

                            if (!taxRates.ContainsKey(taxRate))
                                taxRates.Add(taxRate, caTax);
                            else
                                taxRates[taxRate] += caTax;
                        }
                    }
                }
            }

            if (subTotalWithoutDiscountExclTax < decimal.Zero)
                subTotalWithoutDiscountExclTax = decimal.Zero;

            if (subTotalWithoutDiscountInclTax < decimal.Zero)
                subTotalWithoutDiscountInclTax = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalWithoutDiscountInclTax = await _priceCalculationService.RoundPriceAsync(subTotalWithoutDiscountInclTax);
                subTotalWithoutDiscountExclTax = await _priceCalculationService.RoundPriceAsync(subTotalWithoutDiscountExclTax);
            }

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            (discountAmountExclTax, appliedDiscounts) = await GetOrderSubtotalDiscountAsync(customer, subTotalWithoutDiscountExclTax);
            if (subTotalWithoutDiscountExclTax < discountAmountExclTax)
                discountAmountExclTax = subTotalWithoutDiscountExclTax;
            discountAmountInclTax = discountAmountExclTax;

            //subtotal with discount (excl tax)
            subTotalWithDiscountExclTax = subTotalWithoutDiscountExclTax - discountAmountExclTax;
            subTotalWithDiscountInclTax = subTotalWithDiscountExclTax;

            //add tax for shopping items & checkout attributes
            var tempTaxRates = new Dictionary<decimal, decimal>(taxRates);
            foreach (var kvp in tempTaxRates)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;

                if (taxValue == decimal.Zero)
                    continue;

                //discount the tax amount that applies to subtotal items
                if (subTotalWithoutDiscountExclTax > decimal.Zero)
                {
                    var discountTax = taxRates[taxRate] * (discountAmountExclTax / subTotalWithoutDiscountExclTax);
                    discountAmountInclTax += discountTax;
                    taxValue = taxRates[taxRate] - discountTax;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        taxValue = await _priceCalculationService.RoundPriceAsync(taxValue);
                    taxRates[taxRate] = taxValue;
                }

                //subtotal with discount (incl tax)
                subTotalWithDiscountInclTax += taxValue;
            }

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                discountAmountInclTax = await _priceCalculationService.RoundPriceAsync(discountAmountInclTax);
                discountAmountExclTax = await _priceCalculationService.RoundPriceAsync(discountAmountExclTax);
            }

            if (subTotalWithDiscountExclTax < decimal.Zero)
                subTotalWithDiscountExclTax = decimal.Zero;

            if (subTotalWithDiscountInclTax < decimal.Zero)
                subTotalWithDiscountInclTax = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalWithDiscountExclTax = await _priceCalculationService.RoundPriceAsync(subTotalWithDiscountExclTax);
                subTotalWithDiscountInclTax = await _priceCalculationService.RoundPriceAsync(subTotalWithDiscountInclTax);
            }

            return (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates);
        }

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        /// <param name="restoredCart">Shopping cart</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateOrderTotalsAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            //sub total
            var (subTotalExclTax, subTotalInclTax, subTotalTaxRates, discountAmountExclTax) = await UpdateSubTotalAsync(updateOrderParameters, restoredCart);

            //shipping
            var (shippingTotalExclTax, shippingTotalInclTax, shippingTaxRate) = await UpdateShippingAsync(updateOrderParameters, restoredCart, subTotalInclTax, subTotalExclTax);

            //tax rates
            var taxTotal = await UpdateTaxRatesAsync(subTotalTaxRates, shippingTotalInclTax, shippingTotalExclTax, shippingTaxRate, updateOrderParameters.UpdatedOrder);

            //total
            await UpdateTotalAsync(updateOrderParameters, subTotalExclTax, discountAmountExclTax, shippingTotalExclTax, taxTotal);
        }

        /// <summary>
        /// Gets a value indicating whether shipping is free
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="subTotal">Subtotal amount; pass null to calculate subtotal</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether shipping is free
        /// </returns>
        public virtual async Task<bool> IsFreeShippingAsync(IList<ShoppingCartItem> cart, decimal? subTotal = null)
        {
            //check whether customer is in a customer role with free shipping applied
            var customer = await _customerService.GetCustomerByIdAsync(cart.FirstOrDefault()?.CustomerId ?? 0);

            if (customer != null && (await _customerService.GetCustomerRolesAsync(customer)).Any(role => role.FreeShipping))
                return true;

            //check whether all shopping cart items and their associated products marked as free shipping
            if (await cart.AllAwaitAsync(async shoppingCartItem => await _shippingService.IsFreeShippingAsync(shoppingCartItem)))
                return true;

            //free shipping over $X
            if (!_shippingSettings.FreeShippingOverXEnabled)
                return false;

            if (!subTotal.HasValue)
            {
                var (_, _, _, subTotalWithDiscount, _) = await GetShoppingCartSubTotalAsync(cart, _shippingSettings.FreeShippingOverXIncludingTax);
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
        /// <param name="applyToPickupInStore">Adjust shipping rate to pickup in store shipping option rate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the adjusted shipping rate. Applied discounts
        /// </returns>
        public virtual async Task<(decimal adjustedShippingRate, List<Discount> appliedDiscounts)> AdjustShippingRateAsync(decimal shippingRate, IList<ShoppingCartItem> cart,
            bool applyToPickupInStore = false)
        {
            //free shipping
            if (await IsFreeShippingAsync(cart))
                return (decimal.Zero, new List<Discount>());

            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();

            //with additional shipping charges
            var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);

            var adjustedRate = shippingRate;

            if (!(applyToPickupInStore && _shippingSettings.AllowPickupInStore && pickupPoint != null && _shippingSettings.IgnoreAdditionalShippingChargeForPickupInStore))
            {
                adjustedRate += await GetShoppingCartAdditionalShippingChargeAsync(cart);
            }

            //discount
            var (discountAmount, appliedDiscounts) = await GetShippingDiscountAsync(customer, adjustedRate);
            adjustedRate -= discountAmount;

            adjustedRate = Math.Max(adjustedRate, decimal.Zero);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = await _priceCalculationService.RoundPriceAsync(adjustedRate);

            return (adjustedRate, appliedDiscounts);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total
        /// </returns>
        public virtual async Task<decimal?> GetShoppingCartShippingTotalAsync(IList<ShoppingCartItem> cart)
        {
            var includingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return (await GetShoppingCartShippingTotalAsync(cart, includingTax)).shippingTotal;
        }


        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Applied tax rate. Applied discounts
        /// </returns>
        public virtual async Task<(decimal? shippingTotal, decimal taxRate, List<Discount> appliedDiscounts)> GetShoppingCartShippingTotalAsync(IList<ShoppingCartItem> cart, bool includingTax)
        {
            decimal? shippingTotal = null;
            var appliedDiscounts = new List<Discount>();
            var taxRate = decimal.Zero;

            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);

            var isFreeShipping = await IsFreeShippingAsync(cart);
            if (isFreeShipping)
                return (decimal.Zero, taxRate, appliedDiscounts);

            ShippingOption shippingOption = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (customer != null)
                shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);

            if (shippingOption != null)
            {
                //use last shipping option (get from cache)
                (shippingTotal, appliedDiscounts) = await AdjustShippingRateAsync(shippingOption.Rate, cart, shippingOption.IsPickupInStore);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer);

                var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(await _workContext.GetCurrentCustomerAsync(), store.Id);
                if (!shippingRateComputationMethods.Any() && !_shippingSettings.AllowPickupInStore)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];

                    var shippingOptionRequests = (await _shippingService.CreateShippingOptionRequestsAsync(cart,
                        shippingAddress,
                        store.Id)).shipmentPackages;

                    decimal? fixedRate = null;
                    foreach (var shippingOptionRequest in shippingOptionRequests)
                    {
                        //calculate fixed rates for each request-package
                        var fixedRateTmp = await shippingRateComputationMethod.GetFixedRateAsync(shippingOptionRequest);
                        if (!fixedRateTmp.HasValue)
                            continue;

                        if (!fixedRate.HasValue)
                            fixedRate = decimal.Zero;

                        fixedRate += fixedRateTmp.Value;
                    }

                    if (fixedRate.HasValue)
                    {
                        //adjust shipping rate
                        (shippingTotal, appliedDiscounts) = await AdjustShippingRateAsync(fixedRate.Value, cart);
                    }
                }
            }

            if (!shippingTotal.HasValue)
                return (null, taxRate, appliedDiscounts);

            if (shippingTotal.Value < decimal.Zero)
                shippingTotal = decimal.Zero;

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotal = await _priceCalculationService.RoundPriceAsync(shippingTotal.Value);

            decimal? shippingTotalTaxed;

            (shippingTotalTaxed, taxRate) = await _taxService.GetShippingPriceAsync(shippingTotal.Value,
                includingTax,
                customer);

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotalTaxed = await _priceCalculationService.RoundPriceAsync(shippingTotalTaxed.Value);

            return (shippingTotalTaxed, taxRate, appliedDiscounts);
        }

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Applied tax rate. Applied discounts
        /// </returns>
        public virtual async Task<(decimal? shippingTotalInclTax, decimal? shippingTotaExclTax, decimal taxRate, List<Discount> appliedDiscounts)> GetShoppingCartShippingTotalsAsync(IList<ShoppingCartItem> cart)
        {
            decimal? shippingTotal = null;
            var appliedDiscounts = new List<Discount>();
            var taxRate = decimal.Zero;

            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);

            var isFreeShipping = await IsFreeShippingAsync(cart);
            if (isFreeShipping)
                return (decimal.Zero, decimal.Zero, taxRate, appliedDiscounts);

            ShippingOption shippingOption = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (customer != null)
                shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);

            if (shippingOption != null)
            {
                //use last shipping option (get from cache)
                (shippingTotal, appliedDiscounts) = await AdjustShippingRateAsync(shippingOption.Rate, cart, shippingOption.IsPickupInStore);
            }
            else
            {
                //use fixed rate (if possible)
                Address shippingAddress = null;
                if (customer != null)
                    shippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer);

                var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync(await _workContext.GetCurrentCustomerAsync(), store.Id);
                if (!shippingRateComputationMethods.Any() && !_shippingSettings.AllowPickupInStore)
                    throw new NopException("Shipping rate computation method could not be loaded");

                if (shippingRateComputationMethods.Count == 1)
                {
                    var shippingRateComputationMethod = shippingRateComputationMethods[0];

                    var shippingOptionRequests = (await _shippingService.CreateShippingOptionRequestsAsync(cart,
                        shippingAddress,
                        store.Id)).shipmentPackages;

                    decimal? fixedRate = null;
                    foreach (var shippingOptionRequest in shippingOptionRequests)
                    {
                        //calculate fixed rates for each request-package
                        var fixedRateTmp = await shippingRateComputationMethod.GetFixedRateAsync(shippingOptionRequest);
                        if (!fixedRateTmp.HasValue)
                            continue;

                        if (!fixedRate.HasValue)
                            fixedRate = decimal.Zero;

                        fixedRate += fixedRateTmp.Value;
                    }

                    if (fixedRate.HasValue)
                    {
                        //adjust shipping rate
                        (shippingTotal, appliedDiscounts) = await AdjustShippingRateAsync(fixedRate.Value, cart);
                    }
                }
            }

            if (!shippingTotal.HasValue)
                return (null, null, taxRate, appliedDiscounts);

            if (shippingTotal.Value < decimal.Zero)
                shippingTotal = decimal.Zero;

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingTotal = await _priceCalculationService.RoundPriceAsync(shippingTotal.Value);

            decimal? shippingTotalTaxedInclTaxt, shippingTotalTaxedExclTaxt;

            (shippingTotalTaxedInclTaxt, taxRate) = await _taxService.GetShippingPriceAsync(shippingTotal.Value,
                true,
                customer);

            (shippingTotalTaxedExclTaxt, _) = await _taxService.GetShippingPriceAsync(shippingTotal.Value,
                false,
                customer);

            //round
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                shippingTotalTaxedInclTaxt = await _priceCalculationService.RoundPriceAsync(shippingTotalTaxedInclTaxt.Value);
                shippingTotalTaxedExclTaxt = await _priceCalculationService.RoundPriceAsync(shippingTotalTaxedExclTaxt.Value);
            }

            return (shippingTotalTaxedInclTaxt, shippingTotalTaxedExclTaxt, taxRate, appliedDiscounts);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total, Tax rates
        /// </returns>
        public virtual async Task<(decimal taxTotal, SortedDictionary<decimal, decimal> taxRates)> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var taxTotalResult = await _taxService.GetTaxTotalAsync(cart, usePaymentMethodAdditionalFee);
            var taxRates = taxTotalResult?.TaxRates ?? new SortedDictionary<decimal, decimal>();
            var taxTotal = taxTotalResult?.TaxTotal ?? decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = await _priceCalculationService.RoundPriceAsync(taxTotal);

            return (taxTotal, taxRates);
        }

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating order total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart total;Null if shopping cart total couldn't be calculated now. Applied gift cards. Applied discount amount. Applied discounts. Reward points to redeem. Reward points amount in primary store currency to redeem
        /// </returns>
        public virtual async Task<(decimal? shoppingCartTotal, decimal discountAmount, List<Discount> appliedDiscounts, List<AppliedGiftCard> appliedGiftCards, int redeemedRewardPoints, decimal redeemedRewardPointsAmount)> GetShoppingCartTotalAsync(IList<ShoppingCartItem> cart,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
        {
            var redeemedRewardPoints = 0;
            var redeemedRewardPointsAmount = decimal.Zero;

            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var paymentMethodSystemName = string.Empty;

            if (customer != null)
            {
                paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            }

            //subtotal without tax
            var (_, _, _, subTotalWithDiscountBase, _) = await GetShoppingCartSubTotalAsync(cart, false);
            //subtotal with discount
            var subtotalBase = subTotalWithDiscountBase;

            //shipping without tax
            var shoppingCartShipping = (await GetShoppingCartShippingTotalAsync(cart, false)).shippingTotal;

            //payment method additional fee without tax
            var paymentMethodAdditionalFeeWithoutTax = decimal.Zero;
            if (usePaymentMethodAdditionalFee && !string.IsNullOrEmpty(paymentMethodSystemName))
            {
                var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart,
                    paymentMethodSystemName);
                paymentMethodAdditionalFeeWithoutTax =
                    (await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee,
                        false, customer)).price;
            }

            //tax
            var shoppingCartTax = (await GetTaxTotalAsync(cart, usePaymentMethodAdditionalFee)).taxTotal;

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
                resultTemp = await _priceCalculationService.RoundPriceAsync(resultTemp);

            //order total discount
            var (discountAmount, appliedDiscounts) = await GetOrderTotalDiscountAsync(customer, resultTemp);

            //sub totals with discount        
            if (resultTemp < discountAmount)
                discountAmount = resultTemp;

            //reduce subtotal
            resultTemp -= discountAmount;

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = await _priceCalculationService.RoundPriceAsync(resultTemp);

            //let's apply gift cards now (gift cards that can be used)
            var appliedGiftCards = new List<AppliedGiftCard>();
            resultTemp = await AppliedGiftCardsAsync(cart, appliedGiftCards, customer, resultTemp);

            if (resultTemp < decimal.Zero)
                resultTemp = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                resultTemp = await _priceCalculationService.RoundPriceAsync(resultTemp);

            if (!shoppingCartShipping.HasValue)
            {
                //we have errors
                return (null, discountAmount, appliedDiscounts, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount);
            }

            var orderTotal = resultTemp;

            //reward points
            (redeemedRewardPoints, redeemedRewardPointsAmount) = await SetRewardPointsAsync(redeemedRewardPoints, redeemedRewardPointsAmount, useRewardPoints, customer, orderTotal);

            orderTotal -= redeemedRewardPointsAmount;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                orderTotal = await _priceCalculationService.RoundPriceAsync(orderTotal);
            return (orderTotal, discountAmount, appliedDiscounts, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount);
        }

        /// <summary>
        /// Calculate payment method fee
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="fee">Fee value</param>
        /// <param name="usePercentage">Is fee amount specified as percentage or fixed value?</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<decimal> CalculatePaymentAdditionalFeeAsync(IList<ShoppingCartItem> cart, decimal fee, bool usePercentage)
        {
            if (!usePercentage || fee <= 0)
                return fee;

            var orderTotalWithoutPaymentFee = (await GetShoppingCartTotalAsync(cart, usePaymentMethodAdditionalFee: false)).shoppingCartTotal ?? 0;
            var result = (decimal)((float)orderTotalWithoutPaymentFee * (float)fee / 100f);

            return result;
        }

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the converted value
        /// </returns>
        public virtual async Task<decimal> ConvertRewardPointsToAmountAsync(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = await _priceCalculationService.RoundPriceAsync(result);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of reward points
        /// </returns>
        public virtual async Task<int> CalculateRewardPointsAsync(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || await _customerService.IsGuestAsync(customer))
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount) * _rewardPointsSettings.PointsForPurchases_Points;
            return points;
        }

        #endregion
    }
}