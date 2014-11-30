using System;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class RoundingHelper
    {
        /// <summary>
        /// Round a product or order total
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <returns>Rounded value</returns>
        public static decimal RoundPrice(decimal value)
        {
            //if (!EngineContext.Current.Resolve<ShoppingCartSettings>().RoundPricesDuringCalculation)
            //    return value;

            //we use this method because
            //some currencies (e.g. hungarian forint) use non-standard rules for rounding
            //you can implement any rounding logic here
            //use EngineContext.Current.Resolve<IWorkContext>() to get current currency

            //round
            value = Math.Round(value, 2);
            return value;
        }
    }
}
