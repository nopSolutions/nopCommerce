using System;
using System.Linq;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping
{
    public static class ShippingExtentions
    {
        public static bool IsShippingRateComputationMethodActive(this IShippingRateComputationMethod srcm,
            ShippingSettings shippingSettings)
        {
            if (srcm == null)
                throw new ArgumentNullException("srcm");

            if (shippingSettings == null)
                throw new ArgumentNullException("shippingSettings");

            if (shippingSettings.ActiveShippingRateComputationMethodSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in shippingSettings.ActiveShippingRateComputationMethodSystemNames)
                if (srcm.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        public static bool CountryRestrictionExists(this ShippingMethod shippingMethod,
            int countryId)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            bool result = shippingMethod.RestrictedCountries.ToList().Find(c => c.Id == countryId) != null;
            return result;
        }


        /// <summary>
        /// Get dimensions
        /// </summary>
        /// <param name="request">Request to get shipping options</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="useCubeRootMethod">A value indicating whether dimensions are calculated based  on cube root of volume</param>
        public static void GetDimensions(this GetShippingOptionRequest request, 
            out decimal width, out decimal length, out decimal height, bool useCubeRootMethod = true)
        {
            if (useCubeRootMethod)
            {
                //cube root of volume
                decimal totalVolume = 0;
                decimal maxProductWidth = 0;
                decimal maxProductLength = 0;
                decimal maxProductHeight = 0;
                foreach (var shoppingCartItem in request.Items)
                {
                    var productVariant = shoppingCartItem.ProductVariant;
                    if (productVariant != null)
                    {
                        totalVolume += shoppingCartItem.Quantity * productVariant.Height * productVariant.Width * productVariant.Length;

                        if (productVariant.Width > maxProductWidth)
                            maxProductWidth = productVariant.Width;
                        if (productVariant.Length > maxProductLength)
                            maxProductLength = productVariant.Length;
                        if (productVariant.Height > maxProductHeight)
                            maxProductHeight = productVariant.Height;
                    }
                }
                decimal dimension = Convert.ToDecimal(Math.Pow(Convert.ToDouble(totalVolume), (double)(1.0 / 3.0)));
                length = width = height = dimension;

                //sometimes we have products with sizes like 1x1x20
                //that's why let's ensure that a maximum dimension is always preserved
                //otherwise, shipping rate computation methods can return low rates
                if (width < maxProductWidth)
                    width = maxProductWidth;
                if (length < maxProductLength)
                    length = maxProductLength;
                if (height < maxProductHeight)
                    height = maxProductHeight;
            }
            else
            {
                //summarize all values (very inaccurate with multiple items)
                width = length = height = decimal.Zero;
                foreach (var shoppingCartItem in request.Items)
                {
                    var productVariant = shoppingCartItem.ProductVariant;
                    if (productVariant != null)
                    {
                        width += productVariant.Width * shoppingCartItem.Quantity;
                        length += productVariant.Length * shoppingCartItem.Quantity;
                        height += productVariant.Height * shoppingCartItem.Quantity;
                    }
                }
            }
        }

        /// <summary>
        /// Gets total width
        /// </summary>
        /// <param name="request">Request to get shipping options</param>
        /// <returns>Total width</returns>
        public static decimal GetTotalWidth(this GetShippingOptionRequest request)
        {
            decimal length, width, height = 0;
            GetDimensions(request, out width, out length, out height);
            return width;
        }

        /// <summary>
        /// Gets total length
        /// </summary>
        /// <param name="request">Request to get shipping options</param>
        /// <returns>Total length</returns>
        public static decimal GetTotalLength(this GetShippingOptionRequest request)
        {
            decimal length, width, height = 0;
            GetDimensions(request, out width, out length, out height);
            return length;
        }

        /// <summary>
        /// Gets total height
        /// </summary>
        /// <param name="request">Request to get shipping options</param>
        /// <returns>Total height</returns>
        public static decimal GetTotalHeight(this GetShippingOptionRequest request)
        {
            decimal length, width, height = 0;
            GetDimensions(request, out width, out length, out height);
            return height;
        }

    }
}
