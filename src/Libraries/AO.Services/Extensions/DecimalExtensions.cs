using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal AdjustPriceToPresentation(this decimal price, string endsWith)
        {
            if(string.IsNullOrEmpty(endsWith) || endsWith.Length != 1)
            {
                endsWith = "9";
            }

            string tmpPrice = price.ToString("F0");
            tmpPrice = tmpPrice.Substring(0, tmpPrice.Length - 1);
            tmpPrice += endsWith;

            price = Convert.ToDecimal(tmpPrice);
            if (price.ToString().EndsWith("0" + endsWith) && price.ToString().Length > 2)
            {
                price -= 10;
            }
            else if (price.ToString().EndsWith("1" + endsWith) && price.ToString().Length > 2)
            {
                price -= 20;
            }
            return price;
        }
    }
}
