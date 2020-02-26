using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial class PriceCalculationService : IPriceCalculationService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PriceCalculationService(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Gets allowed discounts applied to product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<Discount> GetAllowedDiscountsAppliedToProduct(Product product, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            if (!product.HasDiscountsApplied) 
                return allowedDiscounts;

            //we use this property ("HasDiscountsApplied") for performance optimization to avoid unnecessary database calls
            foreach (var discount in _discountService.GetAppliedDiscounts(product))
            {
                if (discount.DiscountType == DiscountType.AssignedToSkus &&
                    _discountService.ValidateDiscount(discount, customer).IsValid)
                    allowedDiscounts.Add(discount);
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to categories
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<Discount> GetAllowedDiscountsAppliedToCategories(Product product, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //load cached discount models (performance optimization)
            foreach (var discount in _discountService.GetAllDiscounts(DiscountType.AssignedToCategories))
            {
                //load identifier of categories with this discount applied to
                var discountCategoryIds = _categoryService.GetAppliedCategoryIds(discount, customer);

                //compare with categories of this product
                var productCategoryIds = new List<int>();
                if (discountCategoryIds.Any())
                {
                    //load identifier of categories of this product
                    var cacheKey = string.Format(NopCatalogCachingDefaults.ProductCategoryIdsModelCacheKey,
                        product.Id,
                        string.Join(",", _customerService.GetCustomerRoleIds(customer)),
                        _storeContext.CurrentStore.Id);

                    productCategoryIds = _cacheManager.Get(cacheKey, () =>
                        _categoryService
                        .GetProductCategoriesByProductId(product.Id)
                        .Select(x => x.CategoryId)
                        .ToList());
                }

                foreach (var categoryId in productCategoryIds)
                {
                    if (!discountCategoryIds.Contains(categoryId)) 
                        continue;

                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        _discountService.ValidateDiscount(discount, customer).IsValid)
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to manufacturers
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<Discount> GetAllowedDiscountsAppliedToManufacturers(Product product, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            foreach (var discount in _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers))
            {
                //load identifier of manufacturers with this discount applied to
                var discountManufacturerIds = _manufacturerService.GetAppliedManufacturerIds(discount, customer);

                //compare with manufacturers of this product
                var productManufacturerIds = new List<int>();
                if (discountManufacturerIds.Any())
                {
                    //load identifier of manufacturers of this product
                    var cacheKey = string.Format(NopCatalogCachingDefaults.ProductManufacturerIdsModelCacheKey,
                        product.Id,
                        string.Join(",", _customerService.GetCustomerRoleIds(customer)),
                        _storeContext.CurrentStore.Id);
                    productManufacturerIds = _cacheManager.Get(cacheKey, () =>
                        _manufacturerService
                        .GetProductManufacturersByProductId(product.Id)
                        .Select(x => x.ManufacturerId)
                        .ToList());
                }

                foreach (var manufacturerId in productManufacturerIds)
                {
                    if (!discountManufacturerIds.Contains(manufacturerId)) 
                        continue;

                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        _discountService.ValidateDiscount(discount, customer).IsValid)
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected virtual IList<Discount> GetAllowedDiscounts(Product product, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //discounts applied to products
            foreach (var discount in GetAllowedDiscountsAppliedToProduct(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to categories
            foreach (var discount in GetAllowedDiscountsAppliedToCategories(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to manufacturers
            foreach (var discount in GetAllowedDiscountsAppliedToManufacturers(product, customer))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="productPriceWithoutDiscount">Already calculated product price without discount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Discount amount</returns>
        protected virtual decimal GetDiscountAmount(Product product,
            Customer customer,
            decimal productPriceWithoutDiscount,
            out List<Discount> appliedDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            appliedDiscounts = new List<Discount>();
            var appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to products with price entered by a customer
            if (product.CustomerEntersPrice)
                return appliedDiscountAmount;

            //discounts are disabled
            if (_catalogSettings.IgnoreDiscounts)
                return appliedDiscountAmount;

            var allowedDiscounts = GetAllowedDiscounts(product, customer);

            //no discounts
            if (!allowedDiscounts.Any())
                return appliedDiscountAmount;

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, productPriceWithoutDiscount, out appliedDiscountAmount);
            return appliedDiscountAmount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product,
            Customer customer,
            decimal additionalCharge = decimal.Zero,
            bool includeDiscounts = true,
            int quantity = 1)
        {
            return GetFinalPrice(product, customer, additionalCharge, includeDiscounts,
                quantity, out _, out _);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product,
            Customer customer,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            out decimal discountAmount,
            out List<Discount> appliedDiscounts)
        {
            return GetFinalPrice(product, customer,
                additionalCharge, includeDiscounts, quantity,
                null, null,
                out discountAmount, out appliedDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product,
            Customer customer,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount,
            out List<Discount> appliedDiscounts)
        {
            return GetFinalPrice(product, customer, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate, out discountAmount, out appliedDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="overriddenProductPrice">Overridden product price. If specified, then it'll be used instead of a product price. For example, used with product attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscounts">Applied discounts</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Product product,
            Customer customer,
            decimal? overriddenProductPrice,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate,
            out decimal discountAmount,
            out List<Discount> appliedDiscounts)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cacheKey = string.Format(NopCatalogCachingDefaults.ProductPriceModelCacheKey,
                product.Id,
                overriddenProductPrice?.ToString(CultureInfo.InvariantCulture),
                additionalCharge.ToString(CultureInfo.InvariantCulture),
                includeDiscounts,
                quantity,
                string.Join(",", _customerService.GetCustomerRoleIds(customer)),
                _storeContext.CurrentStore.Id);

            var cacheTime = _catalogSettings.CacheProductPrices ? 60 : 0;
            //we do not cache price for rental products
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (product.IsRental)
                cacheTime = 0;

            decimal rezPrice;
            (rezPrice, discountAmount, appliedDiscounts) = _cacheManager.Get(cacheKey, () =>
            {
                var discounts = new List<Discount>();
                var appliedDiscountAmount = decimal.Zero;

                //initial price
                var price = overriddenProductPrice ?? product.Price;

                //tier prices
                var tierPrice = _productService.GetPreferredTierPrice(product, customer, _storeContext.CurrentStore.Id, quantity);
                if (tierPrice != null)
                    price = tierPrice.Price;

                //additional charge
                price += additionalCharge;

                //rental products
                if (product.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price *= _productService.GetRentalPeriods(product, rentalStartDate.Value, rentalEndDate.Value);

                if (includeDiscounts)
                {
                    //discount
                    var tmpDiscountAmount = GetDiscountAmount(product, customer, price, out var tmpAppliedDiscounts);
                    price -= tmpDiscountAmount;

                    if (tmpAppliedDiscounts?.Any() ?? false)
                    {
                        discounts.AddRange(tmpAppliedDiscounts);
                        appliedDiscountAmount = tmpDiscountAmount;
                    }
                }

                if (price < decimal.Zero)
                    price = decimal.Zero;

                return (price, appliedDiscountAmount, discounts);
            }, cacheTime);

            return rezPrice;
        }

        /// <summary>
        /// Gets the product cost (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>Product cost (one item)</returns>
        public virtual decimal GetProductCost(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cost = product.ProductCost;
            var attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        cost += attributeValue.Cost;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                        if (associatedProduct != null)
                            cost += associatedProduct.ProductCost * attributeValue.Quantity;
                        break;
                    default:
                        break;
                }
            }

            return cost;
        }

        /// <summary>
        /// Get a price adjustment of a product attribute value
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="value">Product attribute value</param>
        /// <param name="customer">Customer</param>
        /// <param name="productPrice">Product price (null for using the base product price)</param>
        /// <returns>Price adjustment</returns>
        public virtual decimal GetProductAttributeValuePriceAdjustment(Product product, ProductAttributeValue value, Customer customer, decimal? productPrice = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var adjustment = decimal.Zero;
            switch (value.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    //simple attribute
                    if (value.PriceAdjustmentUsePercentage)
                    {
                        if (!productPrice.HasValue)
                            productPrice = GetFinalPrice(product, customer);

                        adjustment = (decimal)((float)productPrice * (float)value.PriceAdjustment / 100f);
                    }
                    else
                    {
                        adjustment = value.PriceAdjustment;
                    }

                    break;
                case AttributeValueType.AssociatedToProduct:
                    //bundled product
                    var associatedProduct = _productService.GetProductById(value.AssociatedProductId);
                    if (associatedProduct != null)
                    {
                        adjustment = GetFinalPrice(associatedProduct, _workContext.CurrentCustomer) * value.Quantity;
                    }

                    break;
                default:
                    break;
            }

            return adjustment;
        }

        /// <summary>
        /// Round a product or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>Rounded value</returns>
        public virtual decimal RoundPrice(decimal value, Currency currency = null)
        {
            //we use this method because some currencies (e.g. Gungarian Forint or Swiss Franc) use non-standard rules for rounding
            //you can implement any rounding logic here

            currency = currency ?? _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            return Round(value, currency.RoundingType);
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        public virtual decimal Round(decimal value, RoundingType roundingType)
        {
            //default round (Rounding001)
            var rez = Math.Round(value, 2);
            var fractionPart = (rez - Math.Truncate(rez)) * 10;

            //cash rounding not needed
            if (fractionPart == 0)
                return rez;

            //Cash rounding (details: https://en.wikipedia.org/wiki/Cash_rounding)
            switch (roundingType)
            {
                //rounding with 0.05 or 5 intervals
                case RoundingType.Rounding005Up:
                case RoundingType.Rounding005Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    fractionPart %= 5;
                    if (fractionPart == 0)
                        break;

                    if (roundingType == RoundingType.Rounding005Up)
                        fractionPart = 5 - fractionPart;
                    else
                        fractionPart *= -1;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.10 intervals
                case RoundingType.Rounding01Up:
                case RoundingType.Rounding01Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    if (roundingType == RoundingType.Rounding01Down && fractionPart == 5)
                        fractionPart = -5;
                    else
                        fractionPart = fractionPart < 5 ? fractionPart * -1 : 10 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.50 intervals
                case RoundingType.Rounding05:
                    fractionPart *= 10;
                    fractionPart = fractionPart < 25 ? fractionPart * -1 : fractionPart < 50 || fractionPart < 75 ? 50 - fractionPart : 100 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 1.00 intervals
                case RoundingType.Rounding1:
                case RoundingType.Rounding1Up:
                    fractionPart *= 10;

                    if (roundingType == RoundingType.Rounding1Up && fractionPart > 0)
                        rez = Math.Truncate(rez) + 1;
                    else
                        rez = fractionPart < 50 ? Math.Truncate(rez) : Math.Truncate(rez) + 1;

                    break;
                case RoundingType.Rounding001:
                default:
                    break;
            }

            return rez;
        }

        #endregion
    }
}