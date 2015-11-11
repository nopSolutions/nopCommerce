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
            //we use this method because some currencies (e.g. Gungarian Forint or Swiss Franc) use non-standard rules for rounding
            //you can implement any rounding logic here
            //use EngineContext.Current.Resolve<IWorkContext>() to get current currency

            //using Swiss Franc (CHF)? just uncomment the line below
            //return Math.Round(value * 20, 0) / 20;

            //default round
            return Math.Round(value, 2);
        }
    }
}
