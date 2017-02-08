using System;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;

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

            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            return value.Round(workContext.WorkingCurrency.RoundingType);
        }

        /// <summary>
        /// Round a product or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency</param>
        /// <returns>Rounded value</returns>
        public static decimal RoundPrice(decimal value, Currency currency)
        {
            return value.Round(currency.RoundingType);
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        public static decimal Round(this decimal value, RoundingType roundingType)
        {
            //default round (Rounding001)
            var rez = Math.Round(value, 2);
            decimal t;

            //Cash rounding (details: https://en.wikipedia.org/wiki/Cash_rounding)
            switch (roundingType)
            {
                //rounding with 0.05 or 5 intervals
                case RoundingType.Rounding005Up:
                case RoundingType.Rounding005Down:
                    t = (rez - Math.Truncate(rez)) * 10;
                    t = (t - Math.Truncate(t)) * 10;

                    if (roundingType == RoundingType.Rounding005Down)
                        t = t >= 5 ? 5 - t : t * -1;
                    else
                        t = t >= 5 ? 10 - t : 5 - t;

                    rez += t / 100;
                    break;
                //rounding with 0.10 intervals
                case RoundingType.Rounding01Up:
                case RoundingType.Rounding01Down:
                    t = (rez - Math.Truncate(rez)) * 10;
                    t = (t - Math.Truncate(t)) * 10;

                    if (roundingType == RoundingType.Rounding01Down && t == 5)
                        t = -5;
                    else
                        t = t < 5 ? t * -1 : 10 - t;

                    rez += t / 100;
                    break;
                //rounding with 0.50 intervals
                case RoundingType.Rounding05:
                    t = (rez - Math.Truncate(rez)) * 100;
                    t = t < 25 ? t * -1 : t < 50 || t < 75 ? 50 - t : 100 - t;

                    rez += t / 100;
                    break;
                //rounding with 1.00 intervals
                case RoundingType.Rounding1:
                case RoundingType.Rounding1Up:
                    t = (rez - Math.Truncate(rez)) * 100;

                    if (roundingType == RoundingType.Rounding1Up && t > 0)
                        rez = Math.Truncate(rez) + 1;
                    else
                        rez = t < 50 ? Math.Truncate(rez) : Math.Truncate(rez) + 1;

                    break;
                case RoundingType.Rounding001:
                default:
                    break;
            }

            return rez;
        }
    }
}
